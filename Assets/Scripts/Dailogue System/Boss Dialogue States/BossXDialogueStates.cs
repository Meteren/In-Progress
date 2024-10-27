using AdvancedStateHandling;
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
public class FirstEncounterToSay : BaseBossDialogueState
{
    DialogueContainer firstEncounterToSay;

    public FirstEncounterToSay(BossX bossX, DialogueContainer firstEncounterToSay, GameObject panel) : base(bossX,panel)
    {
        this.firstEncounterToSay = firstEncounterToSay;
    }

    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = firstEncounterToSay.dialogues[currentDialogueIndex];
        bossX.StartCoroutine(WriteTextOneByOne(dialogue));
    }
    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
        DialogBoxController(firstEncounterToSay,out bool isReady,firstEncounterToSay.isInControl);
        bossX.firstEncounterReady = isReady;

    }

}


public class InSpecialOneToSay : BaseBossDialogueState
{
    DialogueContainer inSpecialOneToSay;
    public InSpecialOneToSay(BossX bossX, DialogueContainer inSpecialOneToSay, GameObject panel) : base(bossX, panel)
    {
        this.inSpecialOneToSay = inSpecialOneToSay;
    }
    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = inSpecialOneToSay.dialogues[currentDialogueIndex];
        bossX.StartCoroutine(WriteTextOneByOne(dialogue));
        
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
        DialogBoxController(inSpecialOneToSay,out bool isReady,inSpecialOneToSay.isInControl);
        bossX.inSpecialOneToSayIsReady = isReady;
    }
}

public class InSpecialTwoToSay : BaseBossDialogueState
{
    DialogueContainer inSpecialTwoToSay;
    public InSpecialTwoToSay(BossX bossX, DialogueContainer inSpecialTwoToSay, GameObject panel) : base(bossX, panel)
    {
        this.inSpecialTwoToSay = inSpecialTwoToSay;
    }

    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = inSpecialTwoToSay.dialogues[currentDialogueIndex];
        bossX.StartCoroutine(WriteTextOneByOne(dialogue));
    }

    public override void OnExit()
    {
        base.OnExit();
    }


    public override void Update()
    {
        base.Update();
        DialogBoxController(inSpecialTwoToSay,out bool isReady,inSpecialTwoToSay.isInControl);
        bossX.inSpecialTwoToSayIsReady = isReady;
    }

}

public class InDeathToSay : BaseBossDialogueState, IState
{
    DialogueContainer inDeathToSay;
    public InDeathToSay(DialogueContainer inDeathToSay, BossX bossX, GameObject panel) : base(bossX, panel)
    {
        this.inDeathToSay = inDeathToSay;
    }

    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = inDeathToSay.dialogues[currentDialogueIndex];
        bossX.StartCoroutine(WriteTextOneByOne(dialogue));

    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
        DialogBoxController(inDeathToSay, out bool isReady, inDeathToSay.isInControl);
        bossX.inDeathToSayIsReady = isReady;
    }
}

public class InCharacterDeathToSay : BaseBossDialogueState, IState
{
    DialogueContainer inCharacterDeathToSay;
    public InCharacterDeathToSay(DialogueContainer inCharacterDeathToSay,BossX bossX, GameObject panel) : base(bossX, panel)
    {
        this.inCharacterDeathToSay = inCharacterDeathToSay;
    }


    public override void OnStart()
    {
        base.OnStart();
        dialogue.maxVisibleCharacters = 0;
        dialogue.text = inCharacterDeathToSay.dialogues[currentDialogueIndex];
        bossX.StartCoroutine(WriteTextOneByOne(dialogue));
    }
    public override void OnExit()
    {
        base.OnExit();
    }
    public override void Update()
    {
        base.Update();
        DialogBoxController(inCharacterDeathToSay, out bool isReady, inCharacterDeathToSay.isInControl);
        bossX.inCharacterDeathToSayIsReady = isReady;

    }
}
