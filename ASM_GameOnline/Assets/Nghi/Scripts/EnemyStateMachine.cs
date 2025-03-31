using System.Collections;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private IEnemyState currentState;
    private EnemyController enemy;

    public EnemyStateMachine(EnemyController enemy)
    {
        this.enemy = enemy;
        ChangeState(new PatrolState());
    }

    public void Update()
    {
        currentState?.UpdateState(this);
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState = newState;
        currentState.EnterState(enemy);
    }
}

public interface IEnemyState
{
    void EnterState(EnemyController enemy);
    void UpdateState(EnemyStateMachine stateMachine);
}

public class PatrolState : IEnemyState
{
    private EnemyController enemy;
    private Transform targetPoint;

    public void EnterState(EnemyController enemy)
    {
        this.enemy = enemy;
        targetPoint = enemy.PointA;
        enemy.SetAnimation("Male_SoulBender_Walk");
    }

    public void UpdateState(EnemyStateMachine stateMachine)
    {
        if (enemy.IsPlayerDetected)
        {
            stateMachine.ChangeState(new ChaseState());
            return;
        }

        float speed = enemy.WalkSpeed * (targetPoint.position.x > enemy.transform.position.x ? 1 : -1);
        enemy.Move(speed);

        if (Vector2.Distance(enemy.transform.position, targetPoint.position) < 0.2f)
        {
            targetPoint = (targetPoint == enemy.PointA) ? enemy.PointB : enemy.PointA;
        }
    }
}

public class ChaseState : IEnemyState
{
    private EnemyController enemy;

    public void EnterState(EnemyController enemy)
    {
        this.enemy = enemy;
        enemy.SetAnimation("Male_SoulBender_Run");
    }

    public void UpdateState(EnemyStateMachine stateMachine)
    {
        if (!enemy.IsPlayerDetected)
        {
            stateMachine.ChangeState(new ReturnState());
            return;
        }

        if (enemy.IsInAttackRange)
        {
            stateMachine.ChangeState(new AttackState());
            return;
        }

        float speed = enemy.ChaseSpeed * (enemy.Player.position.x > enemy.transform.position.x ? 1 : -1);
        enemy.Move(speed);
    }
}

public class AttackState : IEnemyState
{
    private EnemyController enemy;
    private EnemyStateMachine stateMachine;

    public void EnterState(EnemyController enemy)
    {
        this.enemy = enemy;
        this.stateMachine = enemy.GetStateMachine(); // Lấy stateMachine từ enemy

        enemy.Stop();
        enemy.StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (enemy.IsInAttackRange)
        {
            enemy.Attack();
            yield return new WaitForSeconds(1.5f); // Chờ animation kết thúc
        }

        if (!enemy.IsInAttackRange)
        {
            stateMachine.ChangeState(new ChaseState()); // Sửa lỗi không tìm thấy stateMachine
        }
    }

    public void UpdateState(EnemyStateMachine stateMachine)
    {
        if (!enemy.IsPlayerDetected)
        {
            stateMachine.ChangeState(new ReturnState());
        }
    }
}

public class ReturnState : IEnemyState
{
    private EnemyController enemy;

    public void EnterState(EnemyController enemy)
    {
        this.enemy = enemy;
        enemy.SetAnimation("Male_SoulBender_Walk");
    }

    public void UpdateState(EnemyStateMachine stateMachine)
    {
        float distanceToA = Vector2.Distance(enemy.transform.position, enemy.PointA.position);
        float distanceToB = Vector2.Distance(enemy.transform.position, enemy.PointB.position);

        Transform targetPoint = (distanceToA < distanceToB) ? enemy.PointA : enemy.PointB;
        float speed = enemy.WalkSpeed * (targetPoint.position.x > enemy.transform.position.x ? 1 : -1);

        enemy.Move(speed);

        if (Vector2.Distance(enemy.transform.position, targetPoint.position) < 0.2f)
        {
            stateMachine.ChangeState(new PatrolState());
        }
    }
}
