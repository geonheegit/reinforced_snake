using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public bool each_step;
    public bool each_cycle;

    List<int> action_list = new List<int>();
    List<int> best_action_list = new List<int>();
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
            if (distance_list[list_count - 1] > distance_list[list_count - 2]) // if(current distance > previous distance)
            {
                reward += 5;
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

            while (trial < 200)
            {
                if (gameObject.transform.position != goal.transform.position)
                {
                    trial += 1;
                    reward -= 1;
                    int random_action = Random.Range(1, 5); // 1 ~ 4 pick // 1:L, 2:U, 3:R, 4:D
                    action_list.Add(random_action);
                    yield return new WaitForSeconds(0f);

                    if (random_action == 1 && gameObject.transform.position.x != -10) // L
                    {
                        Vector3 left_vec = new Vector3(-1, 0, 0);
                        gameObject.transform.position += left_vec;
                        random_action = 0;
                    }
                    else if (random_action == 2 && gameObject.transform.position.y != 4) // U
                    {
                        Vector3 up_vec = new Vector3(0, 1, 0);
                        gameObject.transform.position += up_vec;
                        random_action = 0;
                    }
                    else if (random_action == 3 && gameObject.transform.position.x != 10) // R
                    {
                        Vector3 right_vec = new Vector3(1, 0, 0);
                        gameObject.transform.position += right_vec;
                        random_action = 0;
                    }
                    else if (random_action == 4 && gameObject.transform.position.y != -4) // D
                    {
                        Vector3 down_vec = new Vector3(0, -1, 0);
                        gameObject.transform.position += down_vec;
                        random_action = 0;
                        
                    }
                    Reward_Distance();
                    
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
                    break;
                }
                
            }
            reward_list.Add(reward);
            
            if (reward_list[reward_list.Count - 1] >= best_reward)
            {
                best_reward = reward_list[reward_list.Count - 1]; // if current reward >= previous reward, then best reward = current reward.
                best_action_list = action_list;
            }
            if (each_cycle)
            {
                Debug.Log($"=== CYCLE COMPLETED || " +
                    $"{generation}th generation || " +
                    $"current gen reward: {reward_list[reward_list.Count - 1]} || " +
                    $"previous gen reward: {reward_list[reward_list.Count - 2]} || " +
                    $"best gen reward: {best_reward} ===");
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

            yield return new WaitForSeconds(0.1f);
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
            best_reward = 0;
            reward = 0;
            reward_list.Clear();

            // Start
            StartCoroutine(Train());
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("=== Best Action List ===");
            Debug.Log(string.Join(",", best_action_list));
        }
    }
}
