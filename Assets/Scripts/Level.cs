using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] float cameraOrthoSize = 50f;
    [SerializeField] float pipeWidth = 9.2f;
    [SerializeField] float pipeHeadHeight = 5.2f;
    private const float pipeMoveSpeed = 32f; // tweakable
    private const float spawnPipeXPos = 120f; // tweakable, gotta be positive
    private const float birdXPosition = 0f;

    private static Level instance;

    public static Level GetInstance()
    {
        return instance;
    }

    private List<Pipe> pipeList;
    private int pipesPassedCount;
    private int pipesSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax = 1.8f;
    private float gapSize;
    private bool playerDead = false;

    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        gapSize = 50f;
    }

    private void Start()
    {
        Player.GetInstance().OnDeath += Level_OnDeath;
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
            pipeSpawnTimer += pipeSpawnTimerMax;

            float heightEdgeLimit = 9f; // tweakable
            float totalHeight = cameraOrthoSize * 2f;
            float minHeight = gapSize * 0.5f + heightEdgeLimit;
            float maxHeight = totalHeight - gapSize * 0.5f - heightEdgeLimit;

            float height = Random.Range(minHeight, maxHeight);
            CreateGapPipes(height, gapSize, spawnPipeXPos);
        }
    }

    private void SetGapSize()
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
            bool isToTheRightOfBird = pipe.GetXPosition() > birdXPosition;
            pipe.Move();
            if (isToTheRightOfBird && pipe.GetXPosition() <= birdXPosition) // if pipe is to the right of player && to the left of bird at the same time, middle of pipe
            {
                pipesPassedCount++;
                SoundManager.PlaySound(SoundManager.SoundType.Score);
            }
            if (pipe.GetXPosition() < -spawnPipeXPos)
            {
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
        Transform pipeHead = InstantiatePipeHeadWithPosition(height, xPosition, createOnBottom);
        Transform pipeBody = InstantiatePipeBodyWithPosition(xPosition, createOnBottom);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(pipeWidth, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(pipeWidth, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * 0.5f);

        Pipe pipe = new Pipe(pipeHead, pipeBody);
        pipeList.Add(pipe);
    }

    private Transform InstantiatePipeHeadWithPosition(float height, float xPosition, bool createOnBottom)
    {
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadYPosition = -cameraOrthoSize + height - pipeHeadHeight * 0.5f;
        pipeHeadYPosition = InvertIfOnBottom(xPosition, createOnBottom, pipeHead, pipeHeadYPosition);
        return pipeHead;
    }

    private Transform InstantiatePipeBodyWithPosition(float xPosition, bool createOnBottom)
    {
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyYPosition = -cameraOrthoSize;
        pipeBodyYPosition = InvertIfOnBottom(xPosition, createOnBottom, pipeBody, pipeBodyYPosition);
        return pipeBody;
    }

    private static float InvertIfOnBottom(float xPosition, bool createOnBottom, Transform pipePart, float pipePartYPosition)
    {
        if (!createOnBottom)
        {
            pipePartYPosition *= -1; // inverts pipe body spawn position
            pipePart.localScale = new Vector3(1, -1, 1); // inverts actual pipe body
        }
        pipePart.position = new Vector2(xPosition, pipePartYPosition);
        return pipePartYPosition;
    }

    public int GetPipesPassed()
    {
        return pipesPassedCount / 2;
    }


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
            return pipeHeadTransform.position.x;
        }
        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}
