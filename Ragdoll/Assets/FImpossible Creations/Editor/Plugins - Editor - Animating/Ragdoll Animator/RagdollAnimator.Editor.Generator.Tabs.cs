using FIMSpace.AnimationTools;
using FIMSpace.FEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FProceduralAnimation
{

    public partial class RagdollAnimatorEditor
    {

        public void Tabs_DrawSetup(SerializedProperty sp_ragProcessor, RagdollProcessor proc)
        {

            // Generating buttons etc.
            FGUI_Inspector.FoldHeaderStart(ref proc._EditorDrawBones, "  Bones Setup", FGUI_Resources.BGInBoxStyle, FGUI_Resources.Tex_Bone);

            if (proc._EditorDrawBones)
            {
                GUILayout.Space(4);
                SerializedProperty sp_BaseTransform = sp_ragProcessor.FindPropertyRelative("BaseTransform");

                EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                GUILayout.Space(8);
                EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);

                if (sp_BaseTransform.objectReferenceValue == null)
                    EditorGUILayout.PropertyField(sp_BaseTransform, new GUIContent("Chest (Optional)", sp_BaseTransform.tooltip));
                else
                    EditorGUILayout.PropertyField(sp_BaseTransform);

                sp_BaseTransform.Next(false);

                EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                GUILayout.Space(8);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                if (EditorGUI.EndChangeCheck()) { serializedObject.ApplyModifiedProperties(); if (Get.Parameters.LeftUpperArm != null) Get.Parameters.LeftForeArm = Get.Parameters.LeftUpperArm.GetChild(0); }
                EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                GUILayout.Space(5);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                if (EditorGUI.EndChangeCheck()) { serializedObject.ApplyModifiedProperties(); if (Get.Parameters.RightUpperArm != null) Get.Parameters.RightForeArm = Get.Parameters.RightUpperArm.GetChild(0); }
                EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                GUILayout.Space(8);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                if (EditorGUI.EndChangeCheck()) { serializedObject.ApplyModifiedProperties(); if (Get.Parameters.LeftUpperLeg != null) Get.Parameters.LeftLowerLeg = Get.Parameters.LeftUpperLeg.GetChild(0); }
                EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                GUILayout.Space(5);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                if (EditorGUI.EndChangeCheck()) { serializedObject.ApplyModifiedProperties(); if (Get.Parameters.RightUpperLeg != null) Get.Parameters.RightLowerLeg = Get.Parameters.RightUpperLeg.GetChild(0); }
                EditorGUILayout.PropertyField(sp_BaseTransform); sp_BaseTransform.Next(false);
                GUILayout.Space(2);

                if (Get.ObjectWithAnimator)
                {
                    bool isHuman = false;
                    Animator anim = Get.ObjectWithAnimator.GetComponent<Animator>();
                    if (anim) isHuman = anim.isHuman;

                    if (!isHuman)
                    {
                        EditorGUILayout.HelpBox("Detected Generic or Legacy Rig, you want to try auto-detect limbs? (Verify them)", MessageType.None);

                        if (GUILayout.Button(new GUIContent("  Run Auto-Limb Detection Algorithm\n  <size=10>(Character must contain correct T-Pose)\n(And be Facing it's Z-Axis)</size>", FGUI_Resources.TexWaitIcon), FGUI_Resources.ButtonStyleR, GUILayout.Height(52)))
                        {
                            SkeletonRecognize.SkeletonInfo info = new SkeletonRecognize.SkeletonInfo(Get.ObjectWithAnimator);

                            #region Assigning found bones

                            int assigned = 0;
                            if (info.LeftArms > 0)
                            {
                                if (info.ProbablyLeftArms[0].Count > 2)
                                {
                                    assigned += 2;
                                    Get.Parameters.LeftUpperArm = info.ProbablyLeftArms[0][1];
                                    Get.Parameters.LeftForeArm = info.ProbablyLeftArms[0][2];
                                }
                            }

                            if (info.RightArms > 0)
                            {
                                if (info.ProbablyRightArms[0].Count > 2)
                                {
                                    assigned += 2;
                                    Get.Parameters.RightUpperArm = info.ProbablyRightArms[0][1];
                                    Get.Parameters.RightForeArm = info.ProbablyRightArms[0][2];
                                }
                            }


                            if (info.LeftLegs > 0)
                            {
                                if (info.ProbablyLeftLegs[0].Count > 1)
                                {
                                    assigned += 2;
                                    Get.Parameters.LeftUpperLeg = info.ProbablyLeftLegs[0][0];
                                    Get.Parameters.LeftLowerLeg = info.ProbablyLeftLegs[0][1];
                                }
                            }

                            if (info.RightLegs > 0)
                            {
                                if (info.ProbablyRightLegs[0].Count > 1)
                                {
                                    assigned += 2;
                                    Get.Parameters.RightUpperLeg = info.ProbablyRightLegs[0][0];
                                    Get.Parameters.RightLowerLeg = info.ProbablyRightLegs[0][1];
                                }
                            }

                            if (info.ProbablyHead)
                            {
                                assigned += 1;
                                Get.Parameters.Head = info.ProbablyHead;
                            }

                            if (info.ProbablyHips)
                            {
                                assigned += 1;
                                Get.Parameters.Pelvis = info.ProbablyHips;
                            }

                            if (info.SpineChainLength > 1)
                            {
                                assigned += 2;
                                Get.Parameters.SpineStart = info.ProbablySpineChain[0];

                                int shortSp = info.ProbablySpineChainShort.Count;

                                if (shortSp < 3)
                                    Get.Parameters.Chest = info.ProbablySpineChainShort[1];
                                else
                                    if (shortSp > 2)
                                    Get.Parameters.Chest = info.ProbablySpineChainShort[shortSp - 1];

                                if (Get.Parameters.Chest == Get.Parameters.Head) Get.Parameters.Chest = Get.Parameters.Chest.parent;
                            }

                            if (assigned < 2)
                                EditorUtility.DisplayDialog("Auto Detection Report", "Couldn't detect bones on the current rig!", "Ok");
                            else
                                EditorUtility.DisplayDialog("Auto Detection Report", "Found and Assigned " + assigned + " bones to help out faster setup. Please verify the new added bones", "Ok");

                            #endregion

                        }

                    }

                }
            }

            GUILayout.EndVertical();

            GUILayout.Space(9);

            FGUI_Inspector.FoldHeaderStart(ref proc._EditorDrawGenerator, "  Ragdoll Generator", FGUI_Resources.BGInBoxStyle, FGUI_Resources.Tex_Collider);

            if (proc._EditorDrawGenerator)
            {
                GUILayout.Space(3);
                if (generator == null)
                {
                    generator = new RagdollGenerator();
                    generator.BaseTransform = Get.ObjectWithAnimator != null ? Get.ObjectWithAnimator : Get.transform;
                    generator.SetAllBoneReferences(Get.Parameters.Pelvis, Get.Parameters.SpineStart, Get.Parameters.Chest, Get.Parameters.Head, Get.Parameters.LeftUpperArm, Get.Parameters.LeftForeArm, Get.Parameters.RightUpperArm, Get.Parameters.RightForeArm, Get.Parameters.LeftUpperLeg, Get.Parameters.LeftLowerLeg, Get.Parameters.RightUpperLeg, Get.Parameters.RightLowerLeg);
                }

                generator.Tab_RagdollGenerator(Get.Parameters, true);

            }
            else
            {
                if (generator != null)
                    generator.ragdollTweak = RagdollGenerator.tweakRagd.None;
            }

            GUILayout.EndVertical();

            GUILayout.Space(9);

            FGUI_Inspector.FoldHeaderStart(ref drawAdditionalSettings, "  Additional Settings", FGUI_Resources.BGInBoxStyle, FGUI_Resources.Tex_GearSetup);

            if (drawAdditionalSettings)
            {
                EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);

                GUILayout.Space(3);

                // Hips pin and animate pelvis
                EditorGUILayout.BeginHorizontal();
                DisableOnPlay();
                SerializedProperty sp_ext = sp_ragProcessor.FindPropertyRelative("HipsPin");
                EditorGUILayout.PropertyField(sp_ext); sp_ext.NextVisible(false);
                DisableOnPlay(false);
                GUILayout.FlexibleSpace();
                if (proc.HipsPin == false) EditorGUILayout.PropertyField(sp_ext);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(1);


                SerializedProperty sp_fixRoot = sp_ragProcessor.FindPropertyRelative("FixRootInPelvis");
                SerializedProperty sp_fixRootcpy = sp_fixRoot.Copy();
                sp_fixRoot.Next(false);


                // fix root in pelvis and calibrate
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(sp_fixRootcpy);
                sp_fixRootcpy.Next(false);
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(sp_fixRootcpy);
                EditorGUILayout.EndHorizontal();





                GUILayout.Space(6);
                // Root Bone Field
                GUI.color = new Color(0.85f, 1f, 0.85f, Application.isPlaying ? 0.5f : 1f);
                GUI.backgroundColor = new Color(0.85f, 1f, 0.85f, 1f);
                var sproot = sp_ObjectWithAnimator.Copy(); sproot.Next(false);
                EditorGUILayout.PropertyField(sproot);
                GUILayout.Space(3);
                GUI.color = Color.white;
                GUI.backgroundColor = Color.white;

                DisableOnPlay();

                // Auto destr and pre-generate
                var sp = sp_ObjectWithAnimator.Copy(); sp.Next(false); sp.Next(false);
                EditorGUILayout.BeginHorizontal();
                sp.Next(false); EditorGUILayout.PropertyField(sp);
                sp.Next(false);


                if (Get.RootBone)
                    if (Get.PreGenerateDummy || (Get.Parameters.LeftUpperArm && Get.Parameters.LeftUpperArm.GetComponent<Rigidbody>()))
                    {
                        if (Application.isPlaying) GUI.enabled = false;

                        GUILayout.FlexibleSpace();
                        bool preV = sp.boolValue;
                        EditorGUILayout.PropertyField(sp);

                        if (preV != sp.boolValue)
                        {
                            triggerGenerateRagd = true;
                        }

                        #region Debugging Backup
                        //if (sp.boolValue)
                        //{
                        //    if (GUILayout.Button("Pre Generate Dummy"))
                        //    {
                        //        sp.boolValue = !sp.boolValue;
                        //        Get.Parameters.PreGenerateDummy(Get.ObjectWithAnimator, Get.RootBone);
                        //    }
                        //}
                        //else
                        //{
                        //    GUI.backgroundColor = new Color(0.75f, 0.75f, 0.75f, 1f);
                        //    if (GUILayout.Button("Undo Dummy"))
                        //    {
                        //        sp.boolValue = !sp.boolValue;
                        //        Get.Parameters.RemovePreGeneratedDummy();
                        //    }
                        //    GUI.backgroundColor = Color.white;
                        //}
                        #endregion

                        if (Application.isPlaying) GUI.enabled = true;
                    }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(8);

                // Target container for dummy
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 200;
                sp.Next(false); EditorGUILayout.PropertyField(sp);
                EditorGUIUtility.labelWidth = 0;
                if (Get.TargetParentForRagdollDummy == null)
                {
                    if (GUILayout.Button("Self", GUILayout.Width(40)))
                    {
                        Get.TargetParentForRagdollDummy = Get.transform;
                        EditorUtility.SetDirty(Get);
                    }
                }
                EditorGUILayout.EndHorizontal();
                DisableOnPlay(false);


                GUILayout.EndVertical();
                GUILayout.Space(6);
                FGUI_Inspector.FoldHeaderStart(ref drawCorrectionsSettings, "  Corrections", FGUI_Resources.BGInBoxStyle, FGUI_Resources.Tex_Tweaks);

                if (drawCorrectionsSettings)
                {
                    sp_ext = sp_ragProcessor.FindPropertyRelative("StartAfterTPose");

                    GUILayout.Space(3);
                    EditorGUILayout.BeginHorizontal();
                    DisableOnPlay(); EditorGUILayout.PropertyField(sp_ext);
                    sp_ext.NextVisible(false); EditorGUILayout.PropertyField(sp_ext);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    sp_ext.NextVisible(false); EditorGUILayout.PropertyField(sp_ext); DisableOnPlay(false);
                    sp_ext.NextVisible(false); EditorGUILayout.PropertyField(sp_ext);
                    EditorGUILayout.EndHorizontal();

                    DisableOnPlay();
                    var sp2 = sp_ObjectWithAnimator.Copy(); sp2.Next(false);
                    sp2.Next(false); EditorGUILayout.PropertyField(sp2);

                    GUILayout.Space(3);

                }

                EditorGUILayout.EndVertical();
                GUILayout.Space(4);
            }

            GUILayout.EndVertical();
        }


        public void Tabs_DrawTweaking(SerializedProperty sp_ragProcessor, RagdollProcessor proc)
        {
            if (generator != null) generator.ragdollTweak = RagdollGenerator.tweakRagd.None;

            GUILayout.Space(-4);

            EditorGUILayout.BeginVertical(FGUI_Resources.ViewBoxStyle); // ----------
            SerializedProperty sp_param = sp_ragProcessor.FindPropertyRelative("FreeFallRagdoll");

            RagdollProcessor.Editor_DrawTweakGUI(sp_param, Get.Parameters);

            EditorGUILayout.EndVertical();
        }


        public void Tabs_DrawExtra(SerializedProperty sp_ragProcessor, RagdollProcessor proc)
        {
            SerializedProperty sp_ext = sp_ragProcessor.FindPropertyRelative("ExtendedAnimatorSync");
            EditorGUILayout.PropertyField(sp_ext);

            // Repose
            sp_ext.NextVisible(false); EditorGUILayout.PropertyField(sp_ext);

            GUILayout.Space(5);
            GUILayout.BeginHorizontal(); // Ignore Self
            sp_ext.NextVisible(false); EditorGUILayout.PropertyField(sp_ext);
            GUILayout.FlexibleSpace(); // Mass Multiplier
            sp_ext.NextVisible(false); EditorGUILayout.PropertyField(sp_ext);
            GUILayout.EndHorizontal();

            GUILayout.Space(5); // Phys Material
            sp_ext.NextVisible(false); EditorGUILayout.PropertyField(sp_ext);


            GUILayout.Space(5); // Colliders List
            EditorGUI.indentLevel++;
            //EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxBlankStyle);
            sp_ext.NextVisible(false); EditorGUILayout.PropertyField(sp_ext);
            EditorGUI.indentLevel--;
            //EditorGUILayout.EndVertical();

            GUILayout.Space(5); // Events Receiver
            sp_ext = sp_ragProcessor.FindPropertyRelative("SendCollisionEventsTo");
            EditorGUILayout.PropertyField(sp_ext);
            if (sp_ext.objectReferenceValue != null)
            {
                sp_ext.Next(false); EditorGUILayout.PropertyField(sp_ext);
                EditorGUILayout.HelpBox("The SendCollisionEventsTo game object must have attached component with public methods like:\n'ERagColl(RagdollCollisionHelper c)' or 'ERagCollExit(RagdollCollisionHelper c)' to handle collision events.\nYou can get component 'RagdollCollisionHelper' to identify which limb hitted something.", MessageType.Info);
            }


        }

    }
}
