using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class agent_movement : MonoBehaviour
{
    public GameObject goal;
    private SpriteRenderer rend;

    public int reward;
    public float distance;
    public int trial;
    public int goal_generation; // number of generations of training.
    public int generation;
    public int best_reward;
    public int lowest_trial;

    private int real_random_action; // 1st generation random action.
    private int adjusted_random_action; // after the 1st generation.
    private int current_action;

    // USER INPUT
    public int plus_reward;
    public int minus_reward;
    public int trials; // how many trials in one generation.
    public int base_generation;

    public bool each_step;
    public bool each_cycle;
    // USER INPUT

    List<int> action_list = new List<int>();
    List<int> best_action_list = new List<int>();
    List<int> bbbest_action_list = new List<int>();
    List<float> distance_list = new List<float>();
    List<int> reward_list = new List<int>();

    List<int> best_input_int_action_list = new List<int>(); // int list came from input field.

    public InputField ActionListInput;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    void Reward_Distance() // reward agent depends on the distance from a goal.
    {
        distance = (goal.transform.position - gameObject.transform.position).magnitude;
        distance_list.Add(distance);
        int list_count = distance_list.Count;
        if (list_count >= 2)
        {
            if (distance_list[list_count - 1] >= distance_list[list_count - 2]) // if(current distance > previous distance)
            {
                reward += minus_reward;
            }
            else
            {
                reward += plus_reward;
            }
        }
    }

    IEnumerator Train()
    {
        reward_list.Add(reward); // for non-negative index of list.
        reward_list.Add(reward);

        while (generation < goal_generation)
        {
            gameObject.transform.position = new Vector2(-10, -4);
            generation += 1;
            distance = 0;
            trial = 0;
            reward = 0;
            action_list.Clear(); // clear action list after one generation.
            distance_list.Clear(); // clear distance list after one generation.
            distance_list.Add((goal.transform.position - gameObject.transform.position).magnitude); // for non-negative index of list.

            if (generation <= base_generation) // until "base_generation" tries.
            {

                while (trial < trials)
                {
                    real_random_action = Random.Range(1, 5); // 1 ~ 4 pick // 1:L, 2:U, 3:R, 4:D
                    action_list.Add(real_random_action);
                    current_action = real_random_action;

                    if (gameObject.transform.position != goal.transform.position)
                    {
                        trial += 1;
                        reward -= 1;

                        if (current_action == 1 && gameObject.transform.position.x != -10) // L
                        {
                            Vector3 left_vec = new Vector3(-1, 0, 0);
                            gameObject.transform.position += left_vec;
                            current_action = 0;
                        }
                        else if (current_action == 2 && gameObject.transform.position.y != 4) // U
                        {
                            Vector3 up_vec = new Vector3(0, 1, 0);
                            gameObject.transform.position += up_vec;
                            current_action = 0;
                        }
                        else if (current_action == 3 && gameObject.transform.position.x != 10) // R
                        {
                            Vector3 right_vec = new Vector3(1, 0, 0);
                            gameObject.transform.position += right_vec;
                            current_action = 0;
                        }
                        else if (current_action == 4 && gameObject.transform.position.y != -4) // D
                        {
                            Vector3 down_vec = new Vector3(0, -1, 0);
                            gameObject.transform.position += down_vec;
                            current_action = 0;

                        }
                        Reward_Distance();
                        yield return new WaitForSeconds(0f);

                        if (each_step)
                        {
                            Debug.Log($"{generation}th generation || trial = {trial} || " +
                                $"action: {action_list[action_list.Count - 1]} || " +
                                $"current distance: {distance_list[distance_list.Count - 1]} || " +
                                $"previous distance: {distance_list[distance_list.Count - 2]} || " +
                                $"current gen reward: {reward_list[reward_list.Count - 1]} || " +
                                $"previous gen reward: {reward_list[reward_list.Count - 2]} || " +
                                $"current reward: {reward} || index: {distance_list.Count}");
                        }
                    }
                    else
                    {
                        Debug.Log($"ARRIVED || {generation}th, total trial: {trial}");
                        // edited
                        if (trial < lowest_trial)
                        {
                            bbbest_action_list.Clear();
                            for (int i = 0; i < action_list.Count; i++)
                            {
                                bbbest_action_list.Add(action_list[i]);
                            }
                        }
                        break;
                    }

                }
                // reward update here.
                reward_list.Add(reward);

                int list_count = distance_list.Count;
                if (list_count >= 2)
                {
                    if (distance_list[list_count - 1] >= distance_list[list_count - 2]) // if(current distance > previous distance)
                    {
                        reward += minus_reward;
                    }
                    else
                    {
                        reward += plus_reward;
                    }
                }

                if (reward_list[reward_list.Count - 1] > best_reward)
                {
                    Debug.Log("best rewarded");
                    best_reward = reward_list[reward_list.Count - 1]; // if current reward >= previous reward, then best reward = current reward.
                    best_action_list.Clear();
                    for (int i = 0; i < action_list.Count; i++)
                    {
                        best_action_list.Add(action_list[i]);
                    }
                }

            }

            else // Reinforcement Learning Reward Algorithm.txt || not base_gen try.
            {
                
                List<int> count_list = new List<int>();
                List<int> sorted_count_list = new List<int>();
                List<float> prob_list = new List<float>();
                List<int> direction_list = new List<int>() { 1, 2, 3, 4 }; // L, U, R, D
                List<int> sortedDirections = new List<int>() { 0, 0, 0, 0 };


                // Count the number of L, U, R, D (in order) from the best_action_list.
                for (int i = 1; i < 5; i++)
                {
                    int count = best_action_list.Where(x => x.Equals(i)).Count();
                    // Debug.Log(count);
                    count_list.Add(count);
                }

                for (int i = 0; i < count_list.Count; i++) // sorted_count_list = count_list;
                {
                    sorted_count_list.Add(count_list[i]);
                }
                sorted_count_list.Sort();

                // Sort L, U, R, D in order of number of each direction count and store tham into 'sortedDirections'(List).

                for (int i = 0; i < count_list.Count; i++)
                {
                    int index = sorted_count_list.IndexOf(count_list[i]);
                    // Debug.Log(index);
                    sortedDirections[index] = direction_list[i];
                }

                // Calculate Next L, U, R, D probability (in order) into percentage.
                for (int i = 0; i < 4; i++)
                {
                    float prob_perc = ((float)count_list[i] / (float)trials) * 100;
                    prob_list.Add(prob_perc);
                }

                prob_list.Sort(); // Sort prob_list in order to calculate adjusted action below.

                while (trial < trials)
                {
                    yield return new WaitForSeconds(0.01f);
                    // return adjusted action depends on the probability.
                    int k = Random.Range(0, 100); // pick random number (0 ~ 99)
                    // problem on prob_list => check with debug.

                    if (0 <= k && k < prob_list[0]) // Lowest probability (First index of prob_list)
                    {
                        current_action = sortedDirections[0];
                    }
                    else if (prob_list[0] <= k && k < prob_list[0] + prob_list[1])
                    {
                        current_action = sortedDirections[1];
                    }
                    else if (prob_list[0] + prob_list[1] <= k && k < prob_list[0] + prob_list[1] + prob_list[2])
                    {
                        current_action = sortedDirections[2];
                    }
                    else if (prob_list[0] + prob_list[1] + prob_list[2] <= k && k <= prob_list[0] + prob_list[1] + prob_list[2] + prob_list[3] + 1) // Highest probability (Last index of prob_list)
                    {
                        current_action = sortedDirections[3];
                    }

                    if (gameObject.transform.position != goal.transform.position)
                    {
                        trial += 1;
                        reward -= 1;

                        if (current_action == 1 && gameObject.transform.position.x != -10) // L
                        {
                            Vector3 left_vec = new Vector3(-1, 0, 0);
                            gameObject.transform.position += left_vec;
                            current_action = 0;
                        }
                        else if (current_action == 2 && gameObject.transform.position.y != 4) // U
                        {
                            Vector3 up_vec = new Vector3(0, 1, 0);
                            gameObject.transform.position += up_vec;
                            current_action = 0;
                        }
                        else if (current_action == 3 && gameObject.transform.position.x != 10) // R
                        {
                            Vector3 right_vec = new Vector3(1, 0, 0);
                            gameObject.transform.position += right_vec;
                            current_action = 0;
                        }
                        else if (current_action == 4 && gameObject.transform.position.y != -4) // D
                        {
                            Vector3 down_vec = new Vector3(0, -1, 0);
                            gameObject.transform.position += down_vec;
                            current_action = 0;

                        }
                        Reward_Distance();
                        

                        if (each_step)
                        {
                            Debug.Log($"{generation}th generation || trial = {trial} || " +
                                $"action: {current_action}");
                        }
                    }
                    else
                    {
                        Debug.Log($"ARRIVED || {generation}th, total trial: {trial}");
                        // edited
                        if (trial < lowest_trial)
                        {
                            lowest_trial = trial;

                            bbbest_action_list.Clear();
                            for (int i = 0; i < action_list.Count; i++)
                            {
                                bbbest_action_list.Add(action_list[i]);
                            }
                        }
                        break;
                    }
                }
                // reward update here.
                reward_list.Add(reward);

                if (reward_list[reward_list.Count - 1] > best_reward)
                {
                    Debug.Log("best rewarded");
                    best_reward = reward_list[reward_list.Count - 1]; // if current reward >= previous reward, then best reward = current reward.
                    best_action_list.Clear();
                    for (int i = 0; i < action_list.Count; i++)
                    {
                        best_action_list.Add(action_list[i]);
                    }
                }
            }
            // print cycle progress
            if (each_cycle)
            {
                Debug.Log($"=== CYCLE COMPLETED || " +
                    $"{generation}th generation || " +
                    $"current gen reward: {reward_list[reward_list.Count - 1]} || " +
                    $"previous gen reward: {reward_list[reward_list.Count - 2]} || " +
                    $"best gen reward: {best_reward} ===");


                List<int> count_list = new List<int>();
                for (int i = 1; i < 5; i++)
                {
                    int count = best_action_list.Where(x => x.Equals(i)).Count();
                    count_list.Add(count);
                }
            }

        }
        
    }
    public void TakeInputAndPlay() // take string text from input field and convert the text into int list. Play action after the process.
    {
        string ActionList_In = ActionListInput.GetComponent<InputField>().text;

        List<string> string_list = new List<string>(ActionList_In.Split(","));
        List<int> int_list = new List<int>();

        for (int i = 0; i < string_list.Count; i++)
        {
            int outValue = int.Parse(string_list[i]);
            int_list.Add(outValue);
        }

        best_input_int_action_list = int_list;

        // Playing
        gameObject.transform.position = new Vector2(-10, -4); // Reset Agent Position.
        rend.color = new Color(0, 0, 0);// Reset Agent Color to white.

        StartCoroutine(SlowPlay()); // Replay according to given action list.
    }

    IEnumerator SlowPlay()
    {
        for (int i = 0; i < best_input_int_action_list.Count; i++)
        {
            if (best_input_int_action_list[i] == 1 && gameObject.transform.position.x != -10) // L
            {
                Vector3 left_vec = new Vector3(-1, 0, 0);
                gameObject.transform.position += left_vec;
            }
            else if (best_input_int_action_list[i] == 2 && gameObject.transform.position.y != 4) // U
            {
                Vector3 up_vec = new Vector3(0, 1, 0);
                gameObject.transform.position += up_vec;
            }
            else if (best_input_int_action_list[i] == 3 && gameObject.transform.position.x != 10) // R
            {
                Vector3 right_vec = new Vector3(1, 0, 0);
                gameObject.transform.position += right_vec;
            }
            else if (best_input_int_action_list[i] == 4 && gameObject.transform.position.y != -4) // D
            {
                Vector3 down_vec = new Vector3(0, -1, 0);
                gameObject.transform.position += down_vec;
            }

            yield return new WaitForSeconds(0.02f);
        }
        
        rend.color = new Color(1, 1, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Initialization
            gameObject.transform.position = new Vector2(-10, -4);
            rend.color = new Color(0, 0, 0);
            generation = 0;
            best_reward = -100000;
            reward = 0;
            lowest_trial = trials;
            reward_list.Clear();

            // Start
            StartCoroutine(Train());
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("=== Best Action List ===");
            if (bbbest_action_list.Count != 0)
            {
                Debug.Log(string.Join(",", bbbest_action_list));
            }
            else
            {
                Debug.Log(string.Join(",", best_action_list));
            }
        }
    }
}
