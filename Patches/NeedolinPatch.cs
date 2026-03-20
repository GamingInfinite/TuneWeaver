using System;
using HarmonyLib;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using Silksong.FsmUtil.Actions;
using TuneWeaver.Data;
using UnityEngine;

namespace TuneWeaver.Patches;

[HarmonyPatch(typeof(PlayMakerFSM), nameof(PlayMakerFSM.Start))]
public class NeedolinPatch
{
    [HarmonyPostfix]
    public static void Postfix(PlayMakerFSM __instance)
    {
        if (__instance is { name: "Hero_Hornet(Clone)", FsmName: "Silk Specials" })
        {
            Fsm silkSpecials = __instance.Fsm;
            Fsm? needolinFsm = silkSpecials.GetAction<RunFSM>("Needolin Sub", 2)?.fsmTemplateControl.RunFsm;

            FsmState startNeedolin = needolinFsm.GetState("Start Needolin");
            FsmState startNeedolinProper = needolinFsm.GetState("Start Needolin Proper");
            FsmState needolinCancel = needolinFsm.GetState("Cancelable");
            FsmState setTime = needolinFsm.GetState("Set Silk Drain Time");
            FsmState playNeedolin = needolinFsm.GetState("Play Needolin");

            FsmState TuneWeaver = needolinFsm.AddState("Tune Weaver");

            TuneWeaver.AddTransition("FINISHED", startNeedolinProper.Name);
            setTime.ChangeTransition("FINISHED", TuneWeaver.Name);

            FsmBool atBench = needolinFsm.GetFsmBool("At Bench");
            AudioClip defaultHornetNeedolin =
                (AudioClip)startNeedolinProper.GetAction<StartNeedolinAudioLoop>(6).DefaultClip.Value;

            FsmString needolinClip = needolinFsm.GetFsmString("Play Clip");

            DelegateAction<Action> decideStartAnim = new()
            {
                Method = (action) =>
                {
                    TuneWeaverPlugin.ActivationCheck();
                    SongData? songData = TuneWeaverPlugin.ActiveSongData;
                    if (songData == null)
                    {
                        needolinClip.Value = atBench.Value ? "NeedolinSit Start" : "Needolin Start";
                    }
                    else
                    {
                        if (songData.SitStartAnimName == "" && atBench.Value)
                        {
                            needolinClip.Value = "NeedolinSit Start";
                        }
                        else if (songData.StandStartAnimName == "" && !atBench.Value)
                        {
                            needolinClip.Value = "Needolin Start";
                        }
                        else
                        {
                            needolinClip.Value = atBench.Value ? songData.SitStartAnimName : songData.StandStartAnimName;
                        }
                    }

                    action.Invoke();
                }
            };
            decideStartAnim.Arg = decideStartAnim.Finish;

            DelegateAction<Action> decideMainAnim = new()
            {
                Method = (action) =>
                {
                    SongData? songData = TuneWeaverPlugin.ActiveSongData;
                    if (songData == null)
                    {
                        needolinClip.Value = atBench.Value ? "NeedolinSit Play" : "Needolin Play";
                    }
                    else
                    {
                        if (songData.SitAnimName == "" && atBench.Value)
                        {
                            needolinClip.Value = "NeedolinSit Play";
                        }
                        else if (songData.StandAnimName == "" && !atBench.Value)
                        {
                            needolinClip.Value = "Needolin Play";
                        }
                        else
                        {
                            needolinClip.Value = atBench.Value ? songData.SitAnimName : songData.StandAnimName;
                        }
                    }

                    action.Invoke();
                }
            };
            decideMainAnim.Arg = decideMainAnim.Finish;

            startNeedolin.ReplaceAction(decideStartAnim, 6);
            playNeedolin.ReplaceAction(decideMainAnim, 4);

            TuneWeaver.AddLambdaMethod((finish) =>
            {
                SongData? songData = TuneWeaverPlugin.ActiveSongData;
                if (songData == null)
                {
                    startNeedolinProper.GetAction<StartNeedolinAudioLoop>(6).DefaultClip.Value = defaultHornetNeedolin;
                }
                else
                {
                    AudioClip audio = songData.audioClip;
                    startNeedolinProper.GetAction<StartNeedolinAudioLoop>(6).DefaultClip.Value = audio;
                }

                finish.Invoke();
            });
        }
    }
}