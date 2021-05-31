using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Beat;

public class CursorGameScript : MonoBehaviour
{
    public int currentLevel = 0;

    public GameObject myNinja;
    public GameObject whiteNinja;

    public Animator ninjaAnim;
    public Animator whiteNinjaAnim;
    public Animator[] dummyAnims;
    public TrailRenderer[] dummyTrails;

    public ParticleSystem upParticles;
    public ParticleSystem leftParticles;
    public ParticleSystem rightParticles;
    public ParticleSystem upParticlesSlash;
    public ParticleSystem leftParticlesSlash;
    public ParticleSystem rightParticlesSlash;

    public AudioSource up1Aud;
    public AudioSource up2Aud;
    public AudioSource leftAud;
    public AudioSource rightAud;

    public AudioSource cymbal;
    public Text timerText;
    public Text watchText;

    public GameObject resultsCanvObjs;
    public Text resultsText;
    public Image[] resultsStars;
    public Button[] resultsButtons;

    Vector3 startingClickPos;
    bool dragging = false;
    float slashThresh = 150f;

    bool left = true;
    bool right = true;
    bool up = true;

    public Levels myLevels;

    public Arrow[] unmanipulatedTimes; //needs to be public for the guide repositioner
    List<MiniArrow> beginningTimes = new List<MiniArrow>();
    List<MiniArrow> endingTimes = new List<MiniArrow>();
    int arrowsFiredCounter = 0;
    int arrowsHit = 0;

    public int demoHitCounter = 0; //same

    bool gameActive = true;
    bool demoPhase = true;

    private void Awake()
    {
        myLevels = Instantiate(new Levels());
        GetAndMassageLevelData();
    }

    private void Start()
    {
        ArrowShooterScript.instance.CreateProjectileArray(myLevels.allLevels[currentLevel].Length*2, this);
        resultsCanvObjs.SetActive(true);
        resultsCanvObjs.GetComponent<CanvasGroup>().alpha = 0f;
        StartCoroutine(CymbalIn(true));
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive)
        {
            if (Clock.clok.GetMBT().Measure == unmanipulatedTimes[arrowsFiredCounter].measure && Clock.clok.GetMBT().Beat == unmanipulatedTimes[arrowsFiredCounter].beat && (unmanipulatedTimes[arrowsFiredCounter].tick - Clock.clok.GetMBT().Tick) < 4)
            {
                ArrowShooterScript.instance.ShootArrow(unmanipulatedTimes[arrowsFiredCounter].lane);
                arrowsFiredCounter++;
                if (arrowsFiredCounter > unmanipulatedTimes.Length - 1)
                {
                    gameActive = false;
                    //all arrows fired, wait for the last one to be destroyed
                }
            }
        }

        if (demoPhase)
        {
            if (Clock.clok.GetMBT().Measure == unmanipulatedTimes[demoHitCounter].measure + 1 && Clock.clok.GetMBT().Beat == unmanipulatedTimes[demoHitCounter].beat && (unmanipulatedTimes[demoHitCounter].tick - Clock.clok.GetMBT().Tick) < 4)
            {
                Destroy(ArrowShooterScript.instance.thisLevProjectiles[demoHitCounter]);
                switch (beginningTimes[demoHitCounter].lane)
                {
                    case 0:
                        whiteNinjaAnim.SetTrigger("SlashRight");
                        rightParticles.Emit(200);
                        rightParticlesSlash.Emit(20);
                        rightAud.Play();
                        dummyTrails[0].Clear();
                        dummyTrails[0].emitting = true;
                        dummyTrails[0].Clear();
                        dummyAnims[0].ResetTrigger("DummyRight");
                        dummyAnims[0].SetTrigger("DummyRight");
                        break;
                    case 1:
                        whiteNinjaAnim.SetTrigger("SlashUp");
                        upParticles.Emit(200);
                        upParticlesSlash.Emit(20);
                        up1Aud.Play();
                        up2Aud.Play();
                        dummyTrails[1].Clear();
                        dummyTrails[1].emitting = true;
                        dummyTrails[1].Clear();
                        dummyAnims[1].ResetTrigger("DummyUp");
                        dummyAnims[1].SetTrigger("DummyUp");
                        break;
                    case 2:
                        whiteNinjaAnim.SetTrigger("SlashLeft");
                        leftParticles.Emit(200);
                        leftParticlesSlash.Emit(10);
                        leftAud.Play();
                        dummyTrails[2].Clear();
                        dummyTrails[2].emitting = true;
                        dummyTrails[2].Clear();
                        dummyAnims[2].ResetTrigger("DummyLeft");
                        dummyAnims[2].SetTrigger("DummyLeft");
                        break;
                }
                demoHitCounter++;
                if (demoHitCounter > (unmanipulatedTimes.Length / 2) - 1)
                {
                    demoPhase = false;
                    StartCoroutine(CymbalIn(false));
                }
            }
        }
        else
        {
            //comment out this block if you're using the old swipe demo system
            if (demoHitCounter <= (unmanipulatedTimes.Length - 1))
            {
                if (Clock.clok.GetMBT().Measure == unmanipulatedTimes[demoHitCounter].measure + 1 && Clock.clok.GetMBT().Beat == unmanipulatedTimes[demoHitCounter].beat && (unmanipulatedTimes[demoHitCounter].tick - Clock.clok.GetMBT().Tick) < 4)
                {
                    switch (beginningTimes[demoHitCounter - unmanipulatedTimes.Length/2].lane)
                    {
                        case 0:
                            dummyAnims[0].ResetTrigger("DummyRight");
                            dummyAnims[0].SetTrigger("DummyRight");
                            break;
                        case 1:
                            dummyAnims[1].ResetTrigger("DummyUp");
                            dummyAnims[1].SetTrigger("DummyUp");
                            break;
                        case 2:
                            dummyAnims[2].ResetTrigger("DummyLeft");
                            dummyAnims[2].SetTrigger("DummyLeft");
                            break;
                    }
                    demoHitCounter++;
                }
            }
            if (dragging)
            {
                if (Vector3.Distance(Input.mousePosition, startingClickPos) > slashThresh)
                {
                    //dragging = false; //might need this
                    Vector3 directionOfSlash = startingClickPos - Input.mousePosition;

                    if (Mathf.Abs(directionOfSlash.x) > Mathf.Abs(directionOfSlash.y))
                    {
                        if (directionOfSlash.x < 0)
                        {
                            if (left)
                            {
                                left = false;
                                up = true;
                                right = true;
                                ninjaAnim.SetTrigger("SlashLeft");
                                startingClickPos = Input.mousePosition;
                                //register lefthit
                                int tickcount = (int)(Clock.clok.Time / Clock.clok.TickLength);

                                for (int i = 0; i < beginningTimes.Count; i++)
                                {
                                    if (tickcount > beginningTimes[i].ticks && tickcount < endingTimes[i].ticks && beginningTimes[i].lane == 2)
                                    {
                                        //it hit
                                        arrowsHit++;
                                        leftParticles.Emit(200);
                                        leftParticlesSlash.Emit(10);
                                        leftAud.Play();
                                        if (ArrowShooterScript.instance.thisLevProjectiles[i + beginningTimes.Count] != null)
                                        {
                                            ArrowShooterScript.instance.thisLevProjectiles[i + beginningTimes.Count].GetComponent<ProjectileScript>().EndMe();
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (right)
                            {
                                right = false;
                                up = true;
                                left = true;
                                ninjaAnim.SetTrigger("SlashRight");
                                startingClickPos = Input.mousePosition;
                                //register righthit
                                int tickcount = (int)(Clock.clok.Time / Clock.clok.TickLength);

                                for (int i = 0; i < beginningTimes.Count; i++)
                                {
                                    if (tickcount > beginningTimes[i].ticks && tickcount < endingTimes[i].ticks && beginningTimes[i].lane == 0)
                                    {
                                        //it hit
                                        arrowsHit++;
                                        rightParticles.Emit(200);
                                        rightParticlesSlash.Emit(20);
                                        rightAud.Play();
                                        if (ArrowShooterScript.instance.thisLevProjectiles[i + beginningTimes.Count] != null)
                                        {
                                            ArrowShooterScript.instance.thisLevProjectiles[i + beginningTimes.Count].GetComponent<ProjectileScript>().EndMe();
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (directionOfSlash.y < 0 && up)
                    {
                        up = false;
                        left = true;
                        right = true;
                        ninjaAnim.SetTrigger("SlashUp");
                        startingClickPos = Input.mousePosition;
                        //register uphit
                        int tickcount = (int)(Clock.clok.Time / Clock.clok.TickLength);

                        for (int i = 0; i < beginningTimes.Count; i++)
                        {
                            if (tickcount > beginningTimes[i].ticks && tickcount < endingTimes[i].ticks && beginningTimes[i].lane == 1)
                            {
                                //it hit
                                arrowsHit++;
                                upParticles.Emit(200);
                                upParticlesSlash.Emit(20);
                                up1Aud.Play();
                                up2Aud.Play();
                                if (ArrowShooterScript.instance.thisLevProjectiles[i + beginningTimes.Count] != null)
                                {
                                    ArrowShooterScript.instance.thisLevProjectiles[i + beginningTimes.Count].GetComponent<ProjectileScript>().EndMe();
                                }
                                break;
                            }
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
                left = true;
                up = true;
                right = true;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                startingClickPos = Input.mousePosition;
                dragging = true;
            }
        }
    }

    MiniArrow FromMBTToTicks(Arrow mbt)
    {
        MiniArrow temp = new MiniArrow(0, 0);

        temp.ticks += mbt.tick;
        temp.ticks += (mbt.beat - 1) * 96;
        temp.ticks += ((mbt.measure -1) * 4) * 96;
        temp.lane = mbt.lane;

        return temp;
    }

    Arrow FromTicksToMBT(MiniArrow from)
    {
        Arrow temp = new Arrow(0, 0, 0, 0);
        if (from.ticks == 0) return temp;
        temp.measure = 1 + from.ticks / (4 * 4);
        from.ticks = from.ticks - (temp.measure - 1) * 4 * 4;
        temp.beat = 1 + from.ticks / 4;
        temp.tick = (from.ticks % 4) / 10;
        temp.lane = from.lane;
        return temp;
    }

    void GetAndMassageLevelData()
    {
        //unmanipulatedTimes = myLevels.allLevels[currentLevel];
        int LengthOfCurrentLevel = myLevels.allLevels[currentLevel].Length + 0;
        unmanipulatedTimes = new Arrow[LengthOfCurrentLevel * 2];

        for (int i = 0; i < LengthOfCurrentLevel; i++)
        {
            unmanipulatedTimes[i] = myLevels.allLevels[currentLevel][i];
        }
        for (int i = LengthOfCurrentLevel; i < LengthOfCurrentLevel * 2; i++)
        {
            unmanipulatedTimes[i] = new Arrow(myLevels.allLevels[currentLevel][i-LengthOfCurrentLevel].measure + myLevels.allLevels[currentLevel][LengthOfCurrentLevel-1].measure+1, myLevels.allLevels[currentLevel][i-LengthOfCurrentLevel].beat, myLevels.allLevels[currentLevel][i-LengthOfCurrentLevel].tick, myLevels.allLevels[currentLevel][i-LengthOfCurrentLevel].lane);
        }
        //now we've doubled the number of arrows

        for (int q = LengthOfCurrentLevel; q < LengthOfCurrentLevel * 2; q++)
        {
            MiniArrow tempo = FromMBTToTicks(unmanipulatedTimes[q]);
            tempo.ticks -= 20;
            tempo.ticks += 384; //add one measure
            beginningTimes.Add(tempo);

            tempo = FromMBTToTicks(unmanipulatedTimes[q]);
            tempo.ticks += 40;
            tempo.ticks += 384; //add one measure
            endingTimes.Add(tempo);
        }
        //now we have the time to fire arrows, the beginning time of a hit, and the ending time of a hit
    }

    IEnumerator CymbalIn(bool autoOn)
    {
        int beatcounter = 0;
        if (!autoOn)
        {
            yield return new WaitUntil(() => Clock.clok.GetMBT().Beat == 1);
            beatcounter = 1;
            demoPhase = false;
            CursorTrailScript.me.demoPhase = false;

            watchText.color = Color.green;
            watchText.text = "READY";
        }
        else
        {
            watchText.color = Color.red;
            watchText.text = "WATCH";
        }
        //cymbal.PlayScheduled(Clock.clok.AtMBT(new MBT(9,2,1)));
        int origBeat = Clock.clok.GetMBT().Beat;
        float moveCounter = 0f;

        while (beatcounter < 5f)
        {
            if(Clock.clok.GetMBT().Beat != origBeat)
            {
                beatcounter++;
                origBeat = Clock.clok.GetMBT().Beat;
            }
            timerText.text = "" + Mathf.Clamp(beatcounter, 1, 4);
            //also move ninjas
            if (autoOn)
            {
                myNinja.transform.position = Vector3.Lerp(new Vector3(0, 0, -5f), new Vector3(-6, 0, -5), moveCounter);
                whiteNinja.transform.position = Vector3.Lerp(new Vector3(6, 0, -5f), new Vector3(0, 0, -5), moveCounter);
            } else
            {
                myNinja.transform.position = Vector3.Lerp(new Vector3(-6, 0, -5f), new Vector3(0, 0, -5), moveCounter);
                whiteNinja.transform.position = Vector3.Lerp(new Vector3(0, 0, -5f), new Vector3(6, 0, -5), moveCounter);
            }
            
            moveCounter += Time.deltaTime/ 2f;
            yield return null;
        }

        if (!autoOn)
        {
            watchText.gameObject.SetActive(false);
        }
        timerText.text = "";
        yield return null;
    }

    public void GameEnd()
    {
        StartCoroutine(BringUpEndOfGame());
    }

    public IEnumerator BringUpEndOfGame()
    {
        int thresh = beginningTimes.Count;
        int passThresh = Mathf.RoundToInt(beginningTimes.Count / 2f);
        int greatThresh = Mathf.RoundToInt(beginningTimes.Count / 4f) * 3;
        resultsText.text = "Try again!";

        if(arrowsHit >= passThresh)
        {
            resultsStars[0].color = Color.yellow;
            resultsText.text = "Pass!";
            if(arrowsHit >= greatThresh)
            {
                resultsStars[1].color = Color.yellow;
                resultsText.text = "Great!";
                if(arrowsHit == thresh)
                {
                    resultsStars[2].color = Color.yellow;
                    resultsText.text = "Perfect!";
                }
            }
        }

        while(resultsCanvObjs.GetComponent<CanvasGroup>().alpha < 1f)
        {
            resultsCanvObjs.GetComponent<CanvasGroup>().alpha = resultsCanvObjs.GetComponent<CanvasGroup>().alpha + .01f;
            yield return null;
        }
        foreach(Button buto in resultsButtons)
        {
            buto.enabled = true;
        }

        yield return null;
    }
}



public struct Arrow {
    public int measure;
    public int beat;
    public int tick;

    public byte lane; //0 left, 1 up, 2 right

    public Arrow(int measur, int bea, int tic, byte lan)
    {
        measure = measur;
        beat = bea;
        tick = tic;
        lane = lan;
    }
}

public struct MiniArrow{
    public int ticks;
    public byte lane;

    public MiniArrow(int tic, byte lan)
    {
        ticks = tic;
        lane = lan;
    }
}
