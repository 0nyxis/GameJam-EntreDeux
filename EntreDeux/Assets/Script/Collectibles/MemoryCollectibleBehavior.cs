using UnityEngine;

public class MemoryCollectibleBehavior : MonoBehaviour
{

    [Tooltip("Empty if the memory hasn't been collected")]
    [HideInInspector]
    public Player playerRef;

    //animation variables
    private float timer = 0f;
    private Vector3 startPosition;
    private Vector3 endPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        endPosition = new Vector3(startPosition.x, startPosition.y + 1, startPosition.z);

        //this.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("spriteName");
        //this.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("LiveRajangReaction");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerRef != null)
        {
            //Follow behavior
            FollowPlayer();
        }
        else
        {
            //anim behavior
            AnimationUpdate();
        }
    }

    [Tooltip("Function to call when using a memory")]
    public void ConsumeCollectible()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trigger");
        if (collision.gameObject.name.Contains("Player") && playerRef == null)
        {
            //Add it to the list the player has (component or inScript) so it can be affected by the follow player script 
            ///uncomment this (will need a memoryCollectibleList in the player)
            ///playerRef = collision.gameObject.GetComponent<Player>();
            ///playerRef.MemoryList.Add(this);
        }
        //Debug.Log("Fumble");
    }

    private void AnimationUpdate()
    {
        timer += Time.deltaTime;
        float t = (Mathf.Sin(timer) + 1) / 2.0f;
        //float t = Mathf.PingPong(timer * 0.5f, 1.0f);
        this.gameObject.transform.position = Vector3.Lerp(startPosition, endPosition, t);
    }

    private void FollowPlayer()
    {
        this.transform.position = new Vector3(playerRef.transform.position.x - 1, playerRef.transform.position.y + 1, playerRef.transform.position.z);
    }
}
