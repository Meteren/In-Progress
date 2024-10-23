using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoDialogueState : BaseBossDialogueState
{
    
    public NoDialogueState(BossX bossX,GameObject panel) : base(bossX,panel)
    {
    }

    public override void OnStart()
    {
        //base.OnStart();
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
       
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
    }

}
public class FirstToSayState : BaseBossDialogueState
{
    DialogueContainer firstToSay;

    public FirstToSayState(BossX bossX, DialogueContainer firstToSay, GameObject panel) : base(bossX,panel)
    {
        this.firstToSay = firstToSay;
    }

    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = firstToSay.dialogues[currentDialogueIndex];
        bossX.StartCoroutine(WriteTextOneByOne(dialogue));
    }
    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
        DialogBoxController(firstToSay,out bool isReady,firstToSay.isInControl);
        bossX.firstIsReady = isReady;

    }

}


public class InAttackToSay : BaseBossDialogueState
{
    DialogueContainer inAttackToSay;
    public InAttackToSay(BossX bossX, DialogueContainer inAttackToSay, GameObject panel) : base(bossX, panel)
    {
        this.inAttackToSay = inAttackToSay;
    }
    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = inAttackToSay.dialogues[currentDialogueIndex];
        bossX.StartCoroutine(WriteTextOneByOne(dialogue));
        
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
        DialogBoxController(inAttackToSay,out bool isReady,inAttackToSay.isInControl);
        bossX.isInAttackReady = isReady;
    }
}

public class SecondToSayState : BaseBossDialogueState
{
    DialogueContainer secondToSay;
    public SecondToSayState(BossX bossX, DialogueContainer secondToSay, GameObject panel) : base(bossX, panel)
    {
        this.secondToSay = secondToSay;
    }

    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = secondToSay.dialogues[currentDialogueIndex];
        bossX.StartCoroutine(WriteTextOneByOne(dialogue));
    }

    public override void OnExit()
    {
        base.OnExit();
    }


    public override void Update()
    {
        base.Update();
        //DialogBoxController(secondToSay,out bool isReady,dia);
        //bossX.secondIsReady = isReady;
    }

}
