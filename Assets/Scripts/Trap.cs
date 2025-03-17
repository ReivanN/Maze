using UnityEngine;

public class Trap : MonoBehaviour
{
    public TrapType trapType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (trapType == TrapType.SaveMaze)
            {
                MazeManager.Instance.SameMaze();
            }
            else if (trapType == TrapType.NewMaze)
            {
                MazeManager.Instance.NewMaze();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            if (trapType == TrapType.SaveMaze)
            {
                MazeManager.Instance.SameMaze();
            }
            else if (trapType == TrapType.NewMaze)
            {
                MazeManager.Instance.NewMaze();
            }
        }
    }
}

public enum TrapType 
{
    NewMaze,
    SaveMaze
}
