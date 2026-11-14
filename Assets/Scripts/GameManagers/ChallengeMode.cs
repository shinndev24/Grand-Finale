using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengeMode : MonoBehaviour
{
    public List<NpcMovement> characters;
    public SlotPresenter presenter;
    public RoundTimer timer;
    public GenerateNumber generator;
    public Animation curtain;
    public DialogueManager dialogue;

    public GameObject roundPanel;
    public TMP_Text roundText;
    public TMP_Text hintText;
    public float roundScreenTime = 2f;
    public int totalRounds = 7;
    private bool explanationClosed;

    private int playerId;
    private int playerSlot;
    private int startSlot;
    private int safeSlot;
    private int k;
    private int round = 0;
    private bool roundDone;
    private bool revealing;
    public GameObject explanationPanel;

    void Start()
    {
        var player = characters.Find(c => c.isPlayer);
        playerId = player.id;
        foreach (var c in characters)
            if (c.id != playerId) presenter.HideCharacter(c.id);

        startSlot = Random.Range(1, 8);
        StartCoroutine(ShowExplanation());
    }

    void PlayCurtain(string anim)
    {
        curtain.Play(anim);
    }

    IEnumerator StartRound()
    {
        round++;
        revealing = true;
        k = generator.Roll();
        safeSlot = (startSlot - 1 + k - 1) % 7 + 1;
        hintText.text = round == 1
            ? "Counting starts from spot " + startSlot
            : "Counting starts from the last safe spot";

        // Pantalla de ronda
        roundText.text = "Round " + round;
        roundPanel.SetActive(true);
        yield return new WaitForSeconds(roundScreenTime);
        roundPanel.SetActive(false);

        Canvas.ForceUpdateCanvases();
        playerSlot = Random.Range(1, 8);
        presenter.PlaceCharacters(new Dictionary<int, int> { { playerSlot, playerId } });

        PlayCurtain("Curtain up");
        yield return new WaitForSeconds(1f);

        revealing = false;
        roundDone = false;
        timer.StartTimer(ResolveRound);
    }

    public void Choose(int chosenSlot)
    {
        if (roundDone || revealing) return;
        playerSlot = chosenSlot;
        presenter.PlaceCharacters(new Dictionary<int, int> { { playerSlot, playerId } });
        ResolveRound();
    }

    public void Confirm()
    {
        if (roundDone || revealing) return;
        ResolveRound();
    }

    void ResolveRound()
    {
        if (roundDone) return;
        roundDone = true;
        timer.StopTimer();

        if (playerSlot != safeSlot)
        {
            GameResults.won = false;
            GameResults.roundsSurvived = round - 1;
            GameResults.totalRounds = totalRounds;
            SceneManager.LoadScene("EndGame");
            return;
        }

        if (round >= totalRounds)
        {
            GameResults.won = true;
            GameResults.roundsSurvived = round;
            GameResults.totalRounds = totalRounds;
            SceneManager.LoadScene("EndGame");
            return;
        }

        StartCoroutine(SafePhase());
    }

    IEnumerator SafePhase()
    {
        revealing = true;
        startSlot = safeSlot; // la siguiente cuenta empieza aquí

        yield return dialogue.PlayLine(PickLine(), 2.5f);

        PlayCurtain("Curtain down");
        yield return new WaitForSeconds(1f);

        yield return StartRound();
    }

    string PickLine()
    {
        string[] lines =
        {
            "Phew!",
            "Well done, well done, Fabio!",
            "Lucky? Or clever?",
            "The show must go on!"
        };
        return lines[Random.Range(0, lines.Length)];
    }

    IEnumerator ShowExplanation()
    {
        revealing = true; // bloquea los botones de los spots mientras se lee
        explanationClosed = false;
        explanationPanel.SetActive(true);

        while (!explanationClosed)
            yield return null;

        explanationPanel.SetActive(false);
        yield return StartRound();
    }

    public void CloseExplanation()
    {
        explanationClosed = true;
    }
}