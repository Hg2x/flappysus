using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] float cameraOrthoSize = 50f;
    [SerializeField] float pipeWidth = 9.2f;
    [SerializeField] float pipeHeadHeight = 5.2f;
    private const float pipeMoveSpeed = 32f; // tweakable
    private const float spawnPipeXPos = 120f; // tweakable but nah, gotta be positive
    private const float birdXPosition = 0f;

    private static Level instance;

    public static Level GetInstance() // makes this public methods in this level-instance be able to be called in other scripts without typing extra stuff in the other script
    {
        return instance;
    }

    private List<Pipe> pipeList;
    private int pipesPassedCount;
    private int pipesSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax = 1.8f; // tweakable
    private float gapSize;
    private bool playerDead = false;

    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        gapSize = 50f; // todo, make band aid fix for cucked first pipes not having gap into better fix
    }

    private void Start()
    {
        Player.GetInstance().OnDeath += Level_OnDeath; // subscribe to palyer OnDeath event
    }

    private void Level_OnDeath(object sender, System.EventArgs e)
    {
        playerDead = true;
    }

    private void Update()
    {
        if (!playerDead)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
        }
    }

    private void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0)
        {
            pipeSpawnTimer += pipeSpawnTimerMax; //add time til spawning another pipe

            float heightEdgeLimit = 9f; // tweakable
            float totalHeight = cameraOrthoSize * 2f;
            float minHeight = gapSize * 0.5f + heightEdgeLimit;
            float maxHeight = totalHeight - gapSize * 0.5f - heightEdgeLimit;

            float height = Random.Range(minHeight, maxHeight);
            CreateGapPipes(height, gapSize, spawnPipeXPos);
        }
    }

    private void SetGapSize() // simplified version of difficulty implementation in https://youtu.be/b5Wpni9KPik 41:00
    {
        if (pipesSpawned >= 30) gapSize = 30f;
        else if (pipesSpawned >= 20) gapSize = 37f;
        else if (pipesSpawned >= 10) gapSize = 44f;
        else gapSize = 50f;
    }
    private void HandlePipeMovement()
    {
        for (int i = 0; i < pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];
            bool isToTheRightOfBird = pipe.GetXPosition() > birdXPosition; // have to use bool
            pipe.Move();
            if (isToTheRightOfBird && pipe.GetXPosition() <= birdXPosition) // if pipe is to the right of bird && to the left of bird at the same time, middle of pipe
            {
                pipesPassedCount++;
                SoundManager.PlaySound(SoundManager.SoundType.Score);
            }
            if (pipe.GetXPosition() < -spawnPipeXPos) // exact position of pipe spawn position, but the other side of the screen (both off-screen)
            {
                //destroy pipe
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }
    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize * 0.5f, xPosition, true);
        CreatePipe(cameraOrthoSize * 2f - gapY - gapSize * 0.5f, xPosition, false);
        pipesSpawned++;
        SetGapSize();
    }
    private void CreatePipe(float height, float xPosition, bool createOnBottom)
    {
        // CREATES PIPE HEAD
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadYPosition = -cameraOrthoSize + height - pipeHeadHeight * 0.5f;
        if (!createOnBottom)
        {
            pipeHeadYPosition *= -1;  // inverst pipe spawn position, so it's on screen's top
            pipeHead.localScale = new Vector3(1, -1, 1); //inverts actual pipe head
        }
        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);

        // CREATES PIPE BODY
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyYPosition = -cameraOrthoSize;
        if (!createOnBottom)
        {
            pipeBodyYPosition *= -1; // inverts pipe body spawn position
            pipeBody.localScale = new Vector3(1, -1, 1); // inverts actual pipe body
        }
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(pipeWidth, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(pipeWidth, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * 0.5f);

        Pipe pipe = new Pipe(pipeHead, pipeBody);
        pipeList.Add(pipe);
    }

    public int GetPipesPassed()
    {
        return pipesPassedCount / 2; // todo make bandaid fix into better fix
    }

    // REPRESENTS A SINGLE PIPE
    private class Pipe
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
        }
        public void Move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * pipeMoveSpeed * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * pipeMoveSpeed * Time.deltaTime;
        }
        public float GetXPosition()
        {
            return pipeHeadTransform.position.x; //this is fine cause head & body has the same x
        }
        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}
