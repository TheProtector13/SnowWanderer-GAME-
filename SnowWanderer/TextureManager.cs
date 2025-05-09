using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SnowWanderer {
    internal class TextureManager : IDisposable {
        //Fields
        public Texture2D[] Backgrounds { get; init; }

        public Texture2D[] IceBlockANIM1 { get; init; }
        public Texture2D[] IceBlockANIM2 { get; init; }
        public Texture2D[] IceBlockDEFAULT { get; init; }
        public Texture2D[] SnowBlockANIM1 { get; init; }
        public Texture2D[] SnowBlockANIM2 { get; init; }
        public Texture2D[] SnowBlockDEFAULT { get; init; }

        public Texture2D[] FireEffect { get; init; }
        public Texture2D[] TransformEffect { get; init; }

        public Texture2D[] Ruins { get; init; }

        //NPCS
        public Texture2D[] NPC_ICEGOLEM_Dying { get; init; }
        public Texture2D[] NPC_ICEGOLEM_Blinking { get; init; }
        public Texture2D[] NPC_ICEGOLEM_Walking { get; init; }
        public Texture2D[] NPC_STONEGOLEM_Dying { get; init; }
        public Texture2D[] NPC_STONEGOLEM_Blinking { get; init; }
        public Texture2D[] NPC_STONEGOLEM_Walking { get; init; }
        public Texture2D[] NPC_PLAYER_Pickup { get; init; }
        public Texture2D[] NPC_PLAYER_WalkUp { get; init; }
        public Texture2D[] NPC_PLAYER_WalkDown { get; init; }
        public Texture2D[] NPC_PLAYER_WalkLeft { get; init; }
        public Texture2D[] NPC_PLAYER_WalkRight { get; init; }

        //Objective
        public Texture2D[] ObjectiveMarker { get; init; }
        public Texture2D Stick { get; init; }
        public Texture2D Camp { get; init; }

        //Menu
        public Texture2D MenuWindow { get; init; }
        public Texture2D SliderBar { get; init; }
        public Texture2D SliderBall { get; init; }
        public Texture2D[] AddButton { get; init; }
        public Texture2D FieldBG { get; init; }
        public Texture2D[] ControlFOR { get; init; }
        public Texture2D[] ControlIF { get; init; }
        public Texture2D ControlOP { get; init; }
        public Texture2D[] ControlWHILE { get; init; }
        public Texture2D[] MenuNokButton { get; init; }
        public Texture2D[] MenuOkButton { get; init; }
        public Texture2D[] MenuCustomButton { get; init; }

        //Code
        public TextureManager(ContentManager Content)
        {
            //load textures
            Backgrounds = [Content.Load<Texture2D>("BACKGROUND/1"),
                           Content.Load<Texture2D>("BACKGROUND/2"),
                           Content.Load<Texture2D>("BACKGROUND/3"),
                           Content.Load<Texture2D>("BACKGROUND/4")];

            IceBlockANIM1 = [Content.Load<Texture2D>("BLOCKS/S_ICE/S_ICE_ANIM1_1"),
                             Content.Load<Texture2D>("BLOCKS/S_ICE/S_ICE_ANIM1_2")];
            IceBlockANIM2 = [Content.Load<Texture2D>("BLOCKS/S_ICE/S_ICE_ANIM2_1"),
                             Content.Load<Texture2D>("BLOCKS/S_ICE/S_ICE_ANIM2_2")];
            IceBlockDEFAULT = [Content.Load<Texture2D>("BLOCKS/S_ICE/S_ICE_DEFAULT1"),
                               Content.Load<Texture2D>("BLOCKS/S_ICE/S_ICE_DEFAULT2"),
                               Content.Load<Texture2D>("BLOCKS/S_ICE/S_ICE_DEFAULT3"),
                               Content.Load<Texture2D>("BLOCKS/S_ICE/S_ICE_DEFAULT4"),
                               Content.Load<Texture2D>("BLOCKS/S_ICE/S_ICE_DEFAULT5")];

            SnowBlockANIM1 = [Content.Load<Texture2D>("BLOCKS/SNOW/SNOW_ANIM1_1"),
                              Content.Load<Texture2D>("BLOCKS/SNOW/SNOW_ANIM1_2")];
            SnowBlockANIM2 = [Content.Load<Texture2D>("BLOCKS/SNOW/SNOW_ANIM2_1"),
                              Content.Load<Texture2D>("BLOCKS/SNOW/SNOW_ANIM2_2")];
            SnowBlockDEFAULT = [Content.Load<Texture2D>("BLOCKS/SNOW/SNOW_DEFAULT"),
                                Content.Load<Texture2D>("BLOCKS/SNOW/SNOW_DEFAULT2"),
                                Content.Load<Texture2D>("BLOCKS/SNOW/SNOW_DEFAULT3"),
                                Content.Load<Texture2D>("BLOCKS/SNOW/SNOW_DEFAULT4"),
                                Content.Load<Texture2D>("BLOCKS/SNOW/SNOW_DEFAULT5")];

            FireEffect = [Content.Load<Texture2D>("EFFECTS/Explosion_4/Explosion_1"),
                          Content.Load<Texture2D>("EFFECTS/Explosion_4/Explosion_2"),
                          Content.Load<Texture2D>("EFFECTS/Explosion_4/Explosion_3"),
                          Content.Load<Texture2D>("EFFECTS/Explosion_4/Explosion_4"),
                          Content.Load<Texture2D>("EFFECTS/Explosion_4/Explosion_5"),
                          Content.Load<Texture2D>("EFFECTS/Explosion_4/Explosion_6")];
            TransformEffect = [Content.Load<Texture2D>("EFFECTS/Explosion_5/Explosion_1"),
                               Content.Load<Texture2D>("EFFECTS/Explosion_5/Explosion_2"),
                               Content.Load<Texture2D>("EFFECTS/Explosion_5/Explosion_3"),
                               Content.Load<Texture2D>("EFFECTS/Explosion_5/Explosion_4"),
                               Content.Load<Texture2D>("EFFECTS/Explosion_5/Explosion_5"),
                               Content.Load<Texture2D>("EFFECTS/Explosion_5/Explosion_6"),
                               Content.Load<Texture2D>("EFFECTS/Explosion_5/Explosion_7"),
                               Content.Load<Texture2D>("EFFECTS/Explosion_5/Explosion_8"),
                               Content.Load<Texture2D>("EFFECTS/Explosion_5/Explosion_9"),
                               Content.Load<Texture2D>("EFFECTS/Explosion_5/Explosion_10")];

            Ruins = [Content.Load<Texture2D>("RUINS/Snow_ruins1"),
                     Content.Load<Texture2D>("RUINS/Snow_ruins2"),
                     Content.Load<Texture2D>("RUINS/Snow_ruins3"),
                     Content.Load<Texture2D>("RUINS/Snow_ruins4"),
                     Content.Load<Texture2D>("RUINS/Snow_ruins5"),
                     Content.Load<Texture2D>("RUINS/White_ruins1"),
                     Content.Load<Texture2D>("RUINS/White_ruins2"),
                     Content.Load<Texture2D>("RUINS/White_ruins3"),
                     Content.Load<Texture2D>("RUINS/White_ruins4"),
                     Content.Load<Texture2D>("RUINS/White_ruins5")];

            NPC_ICEGOLEM_Dying = [Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_000"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_001"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_002"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_003"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_004"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_005"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_006"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_007"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_008"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_009"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_010"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_011"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_012"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_013"),
                                  Content.Load<Texture2D>("NPCS/Golem_1/Dying/0_Golem_Dying_014")];
            NPC_ICEGOLEM_Blinking = [Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_000"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_001"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_002"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_003"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_004"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_005"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_006"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_007"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_008"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_009"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_010"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_011"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_012"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_013"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_014"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_015"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_016"),
                                     Content.Load<Texture2D>("NPCS/Golem_1/Idle Blinking/0_Golem_Idle Blinking_017")];
            NPC_ICEGOLEM_Walking = [Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_000"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_001"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_002"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_003"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_004"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_005"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_006"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_007"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_008"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_009"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_010"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_011"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_012"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_013"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_014"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_015"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_016"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_017"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_018"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_019"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_020"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_021"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_022"),
                                    Content.Load<Texture2D>("NPCS/Golem_1/Walking/0_Golem_Walking_023")];
            NPC_STONEGOLEM_Dying = [Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_000"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_001"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_002"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_003"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_004"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_005"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_006"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_007"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_008"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_009"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_010"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_011"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_012"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_013"),
                                    Content.Load<Texture2D>("NPCS/Golem_2/Dying/0_Golem_Dying_014")];
            NPC_STONEGOLEM_Blinking = [Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_000"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_001"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_002"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_003"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_004"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_005"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_006"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_007"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_008"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_009"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_010"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_011"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_012"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_013"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_014"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_015"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_016"),
                                       Content.Load<Texture2D>("NPCS/Golem_2/Idle Blinking/0_Golem_Idle Blinking_017")];
            NPC_STONEGOLEM_Walking = [Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_000"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_001"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_002"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_003"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_004"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_005"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_006"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_007"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_008"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_009"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_010"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_011"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_012"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_013"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_014"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_015"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_016"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_017"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_018"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_019"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_020"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_021"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_022"),
                                      Content.Load<Texture2D>("NPCS/Golem_2/Walking/0_Golem_Walking_023")];

            NPC_PLAYER_Pickup = [Content.Load<Texture2D>("PLAYER/PICKUP/tile01"),
                                 Content.Load<Texture2D>("PLAYER/PICKUP/tile02"),
                                 Content.Load<Texture2D>("PLAYER/PICKUP/tile03"),
                                 Content.Load<Texture2D>("PLAYER/PICKUP/tile04"),
                                 Content.Load<Texture2D>("PLAYER/PICKUP/tile05"),
                                 Content.Load<Texture2D>("PLAYER/PICKUP/tile06"),
                                 Content.Load<Texture2D>("PLAYER/PICKUP/tile07"),
                                 Content.Load<Texture2D>("PLAYER/PICKUP/tile08")];
            NPC_PLAYER_WalkUp = [Content.Load<Texture2D>("PLAYER/WALKUP/tile01"),
                                 Content.Load<Texture2D>("PLAYER/WALKUP/tile02"),
                                 Content.Load<Texture2D>("PLAYER/WALKUP/tile03"),
                                 Content.Load<Texture2D>("PLAYER/WALKUP/tile04"),
                                 Content.Load<Texture2D>("PLAYER/WALKUP/tile05"),
                                 Content.Load<Texture2D>("PLAYER/WALKUP/tile06"),
                                 Content.Load<Texture2D>("PLAYER/WALKUP/tile07"),
                                 Content.Load<Texture2D>("PLAYER/WALKUP/tile08"),
                                 Content.Load<Texture2D>("PLAYER/WALKUP/tile09")];
            NPC_PLAYER_WalkDown = [Content.Load<Texture2D>("PLAYER/WALKDOWN/tile01"),
                                   Content.Load<Texture2D>("PLAYER/WALKDOWN/tile02"),
                                   Content.Load<Texture2D>("PLAYER/WALKDOWN/tile03"),
                                   Content.Load<Texture2D>("PLAYER/WALKDOWN/tile04"),
                                   Content.Load<Texture2D>("PLAYER/WALKDOWN/tile05"),
                                   Content.Load<Texture2D>("PLAYER/WALKDOWN/tile06"),
                                   Content.Load<Texture2D>("PLAYER/WALKDOWN/tile07"),
                                   Content.Load<Texture2D>("PLAYER/WALKDOWN/tile08"),
                                   Content.Load<Texture2D>("PLAYER/WALKDOWN/tile09")];
            NPC_PLAYER_WalkLeft = [Content.Load<Texture2D>("PLAYER/WALKLEFT/tile01"),
                                   Content.Load<Texture2D>("PLAYER/WALKLEFT/tile02"),
                                   Content.Load<Texture2D>("PLAYER/WALKLEFT/tile03"),
                                   Content.Load<Texture2D>("PLAYER/WALKLEFT/tile04"),
                                   Content.Load<Texture2D>("PLAYER/WALKLEFT/tile05"),
                                   Content.Load<Texture2D>("PLAYER/WALKLEFT/tile06"),
                                   Content.Load<Texture2D>("PLAYER/WALKLEFT/tile07"),
                                   Content.Load<Texture2D>("PLAYER/WALKLEFT/tile08"),
                                   Content.Load<Texture2D>("PLAYER/WALKLEFT/tile09")];
            NPC_PLAYER_WalkRight = [Content.Load<Texture2D>("PLAYER/WALKRIGHT/tile01"),
                                    Content.Load<Texture2D>("PLAYER/WALKRIGHT/tile02"),
                                    Content.Load<Texture2D>("PLAYER/WALKRIGHT/tile03"),
                                    Content.Load<Texture2D>("PLAYER/WALKRIGHT/tile04"),
                                    Content.Load<Texture2D>("PLAYER/WALKRIGHT/tile05"),
                                    Content.Load<Texture2D>("PLAYER/WALKRIGHT/tile06"),
                                    Content.Load<Texture2D>("PLAYER/WALKRIGHT/tile07"),
                                    Content.Load<Texture2D>("PLAYER/WALKRIGHT/tile08"),
                                    Content.Load<Texture2D>("PLAYER/WALKRIGHT/tile09")];

            ObjectiveMarker = [Content.Load<Texture2D>("OBJECTIVES/tile01"),
                               Content.Load<Texture2D>("OBJECTIVES/tile02"),
                               Content.Load<Texture2D>("OBJECTIVES/tile03"),
                               Content.Load<Texture2D>("OBJECTIVES/tile04"),
                               Content.Load<Texture2D>("OBJECTIVES/tile05"),
                               Content.Load<Texture2D>("OBJECTIVES/tile06"),
                               Content.Load<Texture2D>("OBJECTIVES/tile07"),
                               Content.Load<Texture2D>("OBJECTIVES/tile08")];
            Stick = Content.Load<Texture2D>("OBJECTIVES/stick");
            Camp = Content.Load<Texture2D>("OBJECTIVES/camp");

            MenuWindow = Content.Load<Texture2D>("MENU/default_MENU");
            SliderBar = Content.Load<Texture2D>("MENU/Bar");
            SliderBall = Content.Load<Texture2D>("MENU/Slider");
            AddButton = [Content.Load<Texture2D>("MENU/CONTROL/Add_Button"),
                         Content.Load<Texture2D>("MENU/CONTROL/Add_Button2")];
            FieldBG = Content.Load<Texture2D>("MENU/CONTROL/Num_Box");
            ControlFOR = [Content.Load<Texture2D>("MENU/CONTROL/FOR"),
                          Content.Load<Texture2D>("MENU/CONTROL/FOR_BOX")];
            ControlIF = [Content.Load<Texture2D>("MENU/CONTROL/IF"),
                         Content.Load<Texture2D>("MENU/CONTROL/IF_BOX")];
            ControlOP = Content.Load<Texture2D>("MENU/CONTROL/OPERATION");
            ControlWHILE = [Content.Load<Texture2D>("MENU/CONTROL/WHILE"),
                            Content.Load<Texture2D>("MENU/CONTROL/WHILE_BOX")];
            MenuNokButton = [Content.Load<Texture2D>("MENU/Button_NOK_1"),
                             Content.Load<Texture2D>("MENU/Button_NOK_2")];
            MenuOkButton = [Content.Load<Texture2D>("MENU/Button_OK_1"),
                            Content.Load<Texture2D>("MENU/Button_OK_2")];
            MenuCustomButton = [Content.Load<Texture2D>("MENU/BIG_BUTTON_1"),
                                Content.Load<Texture2D>("MENU/BIG_BUTTON_2")];
        }

        public void Dispose()
        {
            foreach (var texture in Backgrounds)
                texture.Dispose();
            foreach (var texture in IceBlockANIM1)
                texture.Dispose();
            foreach (var texture in IceBlockANIM2)
                texture.Dispose();
            foreach (var texture in IceBlockDEFAULT)
                texture.Dispose();
            foreach (var texture in SnowBlockANIM1)
                texture.Dispose();
            foreach (var texture in SnowBlockANIM2)
                texture.Dispose();
            foreach (var texture in SnowBlockDEFAULT)
                texture.Dispose();
            foreach (var texture in FireEffect)
                texture.Dispose();
            foreach (var texture in TransformEffect)
                texture.Dispose();
            foreach (var texture in Ruins)
                texture.Dispose();
            foreach (var texture in NPC_ICEGOLEM_Dying)
                texture.Dispose();
            foreach (var texture in NPC_ICEGOLEM_Blinking)
                texture.Dispose();
            foreach (var texture in NPC_ICEGOLEM_Walking)
                texture.Dispose();
            foreach (var texture in NPC_STONEGOLEM_Dying)
                texture.Dispose();
            foreach (var texture in NPC_STONEGOLEM_Blinking)
                texture.Dispose();
            foreach (var texture in NPC_STONEGOLEM_Walking)
                texture.Dispose();
            foreach (var texture in NPC_PLAYER_Pickup)
                texture.Dispose();
            foreach (var texture in NPC_PLAYER_WalkUp)
                texture.Dispose();
            foreach (var texture in NPC_PLAYER_WalkDown)
                texture.Dispose();
            foreach (var texture in NPC_PLAYER_WalkLeft)
                texture.Dispose();
            foreach (var texture in NPC_PLAYER_WalkRight)
                texture.Dispose();
            foreach (var texture in ObjectiveMarker)
                texture.Dispose();
            Stick.Dispose();
            Camp.Dispose();
            MenuWindow.Dispose();
            SliderBar.Dispose();
            SliderBall.Dispose();
            foreach (var texture in AddButton)
                texture.Dispose();
            FieldBG.Dispose();
            foreach (var texture in ControlFOR)
                texture.Dispose();
            foreach (var texture in ControlIF)
                texture.Dispose();
            ControlOP.Dispose();
            foreach (var texture in ControlWHILE)
                texture.Dispose();
            foreach (var texture in MenuNokButton)
                texture.Dispose();
            foreach (var texture in MenuOkButton)
                texture.Dispose();
            foreach (var texture in MenuCustomButton)
                texture.Dispose();
        }
    }
}
