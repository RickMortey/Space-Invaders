using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Invaders : MonoBehaviour
{
    public Invader[] prefabs;

    public float spacing = 2.0f;

    // having public variables so we could change them right in Unity
    public int rows = 5;
    public int columns = 11;

    public AnimationCurve speed;
    public Projectile missilePrefab;
    private float padding = 1.0f;

    public float missileAttackRate = 1.0f;
    public int totalInvaders => this.rows * this.columns;

    public int amountKilled { get; private set; }
    public int amountAlive => totalInvaders - amountKilled;
    public float percentKilled => (float)this.amountKilled / (float)this.totalInvaders;
    public float percentAlive => ((float)this.totalInvaders - (float)this.amountKilled) / (float)this.totalInvaders;
    private Vector3 _direction = Vector2.right;

    private void InvaderKilled()
    {
        this.amountKilled++;

        if (this.amountKilled >= this.totalInvaders)
        {
            ScoreManager.instance.PlayerWon();
        }
    }

    private void MissileAttack()
    {
        foreach (Transform invader in this.transform)
        {
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (Random.value < (1.0f / (float)amountAlive))
            {
                Instantiate(this.missilePrefab, invader.position, Quaternion.identity);
                break;
            }
        }
    }
    private void AdvanceRow()
    {
        _direction.x *= -1.0f;

        Vector3 position = this.transform.position;
        position.y -= 1.0f;
        this.transform.position = position;
    }
    private void Awake()
    {
        for (int row = 0; row < this.rows; ++row)
        {
            float widthCentering = spacing * (this.columns - 1);
            float heightCentering = spacing * (this.rows - 1);
            Vector2 centering = new Vector2(-widthCentering / 2, -heightCentering / 2);
            Vector3 rowPosition = new Vector3(centering.x, centering.y + row * spacing, .0f);
            for (int col = 0; col < this.columns; ++col)
            {
                Invader invader = Instantiate(this.prefabs[row], this.transform);
                invader.Killed += InvaderKilled;
                Vector3 position = rowPosition;
                position.x += col * spacing;
                invader.transform.localPosition = position;
            }
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), this.missileAttackRate, this.missileAttackRate);
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.position += this.speed.Evaluate(this.percentKilled) * Time.deltaTime * _direction;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        
        foreach (Transform invader in this.transform)
        {
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (_direction == Vector3.right && invader.position.x >= (rightEdge.x - padding))
            {
                AdvanceRow();
            }
            else if (_direction == Vector3.left && invader.position.x <= (leftEdge.x + padding))
            {
                AdvanceRow();
            }
        }
    }
}