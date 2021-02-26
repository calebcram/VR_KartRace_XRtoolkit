using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaceManager : MonoBehaviour
{
    public int numberOfLap = 1;
    public List<GameObject> checkPoints;

    private int remainingNumberOfLap;
    private int nextCheckPoint = -1;

    public UnityEvent OnStartRace;
    public UnityEvent OnLapReached;
    public UnityEvent OnLastLapReached;
    public UnityEvent OnCheckPointReached;

    public int RemainingNumberOfLap { get => remainingNumberOfLap; set => remainingNumberOfLap = value; }
    public int NextCheckPointToReach { get => nextCheckPoint; set => nextCheckPoint = value; }

    private void Start()
    {
        Initiate();
    }

    public void Initiate()
    {
        remainingNumberOfLap = numberOfLap;
        nextCheckPoint = 0;
        foreach (var item in checkPoints)
        {
            item.SetActive(false);
        }
        checkPoints[0].SetActive(true);

        OnStartRace.Invoke();
    }

    public void NextRing()
    {
        //Check If End
        if (remainingNumberOfLap == 0)
            return;

        if(nextCheckPoint == checkPoints.Count)
        {
            remainingNumberOfLap -= 1;

            if(remainingNumberOfLap == 0)
            {
                Debug.Log("Reached The end Of Lap");
                OnLastLapReached.Invoke();
                return;
            }
            else
            {
                Debug.Log("We need " + remainingNumberOfLap + " more lap.");
                nextCheckPoint = 0;
                OnLapReached.Invoke();
            }
        }

        if(nextCheckPoint >= 0)
        {
            checkPoints[nextCheckPoint].SetActive(false);
        }


        Debug.Log("New CheckPoint Reached " + nextCheckPoint);

        nextCheckPoint = (nextCheckPoint + 1);
        checkPoints[nextCheckPoint%checkPoints.Count].SetActive(true);
        OnCheckPointReached.Invoke();
    }
}
