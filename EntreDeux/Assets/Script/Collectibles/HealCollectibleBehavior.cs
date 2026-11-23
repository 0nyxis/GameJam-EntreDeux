using UnityEngine;

public class HealCollectibleBehavior : MonoBehaviour
{
    public int healAmount = 50;

    //animation variables
    private float timer = 0f;
    private Vector3 startPosition;
    private Vector3 endPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        endPosition = new Vector3(startPosition.x, startPosition.y + 1, startPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trigger");
        if (collision.gameObject.name.Contains("Character"))
        {
            //getComponent that handles the hp of the player and increase them
            ///uncomment this and either make a fct to heal the player or adjust the hp directly
            collision.gameObject.GetComponent<Player>().Heal(healAmount);

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySound("pills");
            }

            Destroy(gameObject);
        }
        //Debug.Log("Fumble");
    }

    private void AnimationUpdate()
    {
        timer += Time.deltaTime;
        float t = (Mathf.Sin(timer) + 1) / 2.0f;
        transform.position = Vector3.Lerp(startPosition, endPosition, t);
    }
}
