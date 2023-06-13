using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agent_movement : MonoBehaviour
{
    public GameObject goal;

    public float current_reward;
    public float previous_reward;
    public float current_distance;
    public float previous_distance;
    public int trial;
    public int random_action; // 1:L, 2:U, 3:R, 4:D

    List<int> list = new List<int>();

    void Start()
    {
        gameObject.transform.position = new Vector2(-10, -4);
        trial = 0;
    }

    void FixedUpdate()
    {
        if (gameObject.transform.position != goal.transform.position)
        {
            trial += 1;
            random_action = Random.Range(1, 5); // 1 ~ 4 pick

            Debug.Log($"trial = {trial} || action: {random_action}");

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
        }
        else
        {
            Debug.Log($"total trial: {trial}");
        }
    }
}
