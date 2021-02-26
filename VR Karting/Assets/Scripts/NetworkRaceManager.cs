using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//STOP THE SPEED at first, and then start everybody once count down has reached zero
//Count down start when number of player exceed threshold which is half the number of player needed in this race
public class NetworkRaceManager : MonoBehaviourPun
{
    //DISPLAY your position in the race when you reach end lap
    public int countDownInSeconds = 15;
    public KartController kartController;
    public TMPro.TextMeshPro countDownText;
    public TMPro.TextMeshPro waitText;
    public TMPro.TextMeshPro leaderboardText;

    //Synchronized over network
    static RaceState raceState;
    private int currentCountDown;
    private List<string> leaderboard = new List<string>();
    public enum RaceState {Wait,CountDown,Start,End}

    public void StartCountDown()
    {
        Debug.Log("START COUNTDOWN");
        SetRaceStateToOthers(RaceState.CountDown, RpcTarget.AllBuffered);
        StartCoroutine(StartCountDownRoutine());
    }

    public void SetRaceStateToOthers(RaceState newRaceState,RpcTarget rpcTarget = RpcTarget.All)
    {
        photonView.RPC("SetRaceStateToOthersRPC", rpcTarget, (int)newRaceState);
    }

    [PunRPC]
    public void SetRaceStateToOthersRPC(int state)
    {
        raceState = (RaceState)state;
    }

    public void SetCountDownToOthers(int newCountDown)
    {
        photonView.RPC("SetCountDownToOthersRPC", RpcTarget.All, newCountDown);
    }

    [PunRPC]
    public void SetCountDownToOthersRPC(int newCountDown)
    {
        currentCountDown = newCountDown;
    }

    IEnumerator StartCountDownRoutine()
    {
        float countDownTimer = countDownInSeconds;

        while (countDownTimer > 0)
        {
            SetCountDownToOthers((int)countDownTimer);
            countDownTimer -= 1;
            yield return new WaitForSeconds(1);
        }

        SetRaceStateToOthers(RaceState.Start);
    }

    public void EndRace()
    {
        Debug.Log("END RACE");
        raceState = RaceState.End;
        string playerName;
        switch (AvatarSelector.currentSelectedAvatarID)
        {
            case 0:
                playerName = "Marcel";
                break;
            case 1:
                playerName = "Louis";
                break;
            case 2:
                playerName = "Pêche";
                break;
            default:
                playerName = "No Name";
                break;
        }

        if (PhotonNetwork.IsConnected)
            photonView.RPC("AddToWinnerListRPC", RpcTarget.All, playerName);
        else
            AddToWinnerListRPC(playerName);
    }


    [PunRPC]
    public void AddToWinnerListRPC(string playerName)
    {
        leaderboard.Add(playerName);
    }


    public void Update()
    {
        if (!PhotonNetwork.InRoom)
            return;

        //The owner of this photonview (which is the first player to enter scene check if there is enough player to start the game)
        if(photonView.IsMine && raceState == RaceState.Wait)
        {
            bool enoughPlayer = CheckIfWeHaveEnoughPlayer();
            if (enoughPlayer)
                StartCountDown();
        }

        UpdateToStateRace();
    }

    public void UpdateToStateRace()
    {
        if (raceState == RaceState.Wait)
        {
            kartController.enabled = false;
            waitText.text = "Waiting for " + GetNumberOfPlayerNeeded().ToString() + "more player(s)";

            countDownText.gameObject.SetActive(false);
            waitText.gameObject.SetActive(true);
            leaderboardText.gameObject.SetActive(false);
        }
        else if (raceState == RaceState.CountDown)
        {
            kartController.enabled = false;
            countDownText.text = currentCountDown.ToString();

            countDownText.gameObject.SetActive(true);
            waitText.gameObject.SetActive(false);
            leaderboardText.gameObject.SetActive(false);
        }
        else if (raceState == RaceState.Start)
        {
            kartController.enabled = true;

            countDownText.gameObject.SetActive(false);
            waitText.gameObject.SetActive(false);
            leaderboardText.gameObject.SetActive(false);
        }
        else if (raceState == RaceState.End)
        {
            kartController.acceleration = 0;

            countDownText.gameObject.SetActive(false);
            waitText.gameObject.SetActive(false);
            leaderboardText.gameObject.SetActive(true);

            leaderboardText.text = LeaderBoardToText();
        }
    }

    private string LeaderBoardToText()
    {
        string text = "LEADER BOARD " + "\n";
        for (int i = 0; i < leaderboard.Count; i++)
        {
            string player = leaderboard[i];
            text += (i+1) + "-" + player + "\n";
        }

        return text;
    }

    //Start the race if the number of player in the room are currently bigger than half the max player
    public bool CheckIfWeHaveEnoughPlayer()
    {
        return GetNumberOfPlayerNeeded() <= 0;
    }

    public int GetNumberOfPlayerNeeded()
    {
        int playerCount = PlayerNumberingExtensions.GetPlayerNumber(PhotonNetwork.LocalPlayer) + 1;
        int playerNeeded = Mathf.FloorToInt(NetworkManager.singleton.currentSelectedRoom.maxPLayer / 2) - playerCount;

        return playerNeeded;
    }
}
