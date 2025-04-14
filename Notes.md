# Notes

## Playtesting Notes

- Huntsman really needs that huh phase, I got fucked with no counterplay

## Misc Changes

- Make pits fall into adjacent rooms from the ceiling, dealing non-lethal fall damage.
- Continue tweaking shop-item balance
- Come up with a better recharge / energy system for tools?
  - Special items that are large and breakable (might have no value either), but provide power crystals when extracted?
- Improve health economy, similar idea to energy crystal, specialised items?
- Make melee weapons have no charge, more skill based somehow?
  - Maybe a durability system where new weapons have to be purchased after old ones break?
- Make the minimap an upgrade, potentially remove entirely.

## Upgrades

- Speed & Stam
  - Reduce stamina use increase from speed upgrades.

### General

- Make upgrades have diminishing effects

## Enemies

- Peeper
- Shadow-child
- Gnome
  - Change to do constant damage to items?
- Apex-predator
- Spewer
- Rugrat (Needs big changes)
  - Maybe it just needs to be held?
    - When being held, it becomes calm, but will become extremely aggressive if it is hurt in any way.
    - If set down for more than 15 seconds or some other amount of time will become aggressive.
    - Will approach players when first spawned, and can only become aggressive after seeing players.
- Animal
- Upscream
  - Just get rid of it OMG I hate this mfer so fucking much
- Hidden
  - Needs more counterplay, especially when players are alone.
  - Maybe it steals equipment from players, collecting it in a stash somewhere?
    - It's just trying to protect the other monsters lol.
    - Can even take the cart potentially?
- Chef (Pretty good already)
- Bowtie (Kinda meh, could use more of a gimmick)
- Mentalist
  - Controls gravity across the entire level, control of gravity gets more powerful as level continues eventually causing game over if too long spent between extractions.
    - Stays in one location where it congregates with other mentalists and is generally passive unless a player takes out a weapon or hits it with an item.
    - Occasionally teleports to another location with other mentalists (once every two or three mins?)
      - Make teleport location be somewhat near a player to increase the chance they are found on large maps?
    - Increased gravity slows players and reduces item and cart mobility.
    - Gravity is completely disabled or even inverted at times, players will take some damage thrown against the ground unless they position well to fall little distance.
    - Gravity control will reduce significantly after every extraction, and increase extra quickly after the last extraction, giving little time for players to escape.
    - Gravity control can be interrupted and charge speed reduced when mentalists are killed.
    - Limit respawns to one or two per extraction, disable despawning.
    - Keep original attack but buff damage and speed when aggroed directly. 
- Banger
- Headman
  - Buff damage and speed to balance new aggro system.
  - Change back to instant aggro after last extraction, maybe reduce damage and speed a little at this point.
- Robe
  - It wants to be part of the team.
    - If players damage items too much around it, it will aggro and chase players for a while, doing pretty high damage.
    - If players hit it with items or the cart it will also aggro.
    - It will occasionally attempt to pick up and move items to help players, if players grab the item while it is doing this it will aggro.
    - Should pathfind to follow a particular player and get in their way a decent bit.
- Huntsman (Fixed!)
- Reaper
  - Only aggro players not moving?
- Clown (Decent already)
- Trudge
  - Really hates extraction points, will approach and destroy them.
    - An indicator that a trudge is approaching an extraction point? (e.g. Unidentified object approaching extraction, distance 10m)
    - Approaches the ship in the same way if no extractions are opened, preventing cheese where players stack stuff near unopened extractions.
    - When an extraction is destroyed, the amount needed is added to the next one? (plus a large fee that is taken directly from player money totall)
    - Will now only aggro players if they attack it or stay directly in front of it for a time.
      - This gives players the choice between extracting quickly and fighting the big boss enemy.

## Items

- C.A.R.T. (Item Cart Medium): Value 9500 - 12000 Max 4 - 4
- POCKET C.A.R.T. (Item Cart Small): Value 3500 - 4500 Max 2 - 2
- Recharge Drone (Item Drone Battery): Value 850 - 1100 Max 1 - 1
- Feather Drone (Item Drone Feather): Value 3500 - 4500 Max 1 - 1
- Indestructible Drone (Item Drone Indestructible): Value 5500 - 7500 Max 1 - 1
- Roll Drone (Item Drone Torque): Value 2000 - 3000 Max 1 - 1
- Zero Gravity Drone (Item Drone Zero Gravity): Value 5500 - 7500 Max 1 - 1
- Extraction Tracker (Item Extraction Tracker): Value 1200 - 2000 Max 1 - 1
- Duct Taped Grenades (Item Grenade Duct Taped): Value 300 - 450 Max 3 - 3
- Grenade (Item Grenade Explosive): Value 500 - 650 Max 3 - 3
- Human Grenade (Item Grenade Human): Value 250 - 500 Max 3 - 3
- Shockwave Grenade (Item Grenade Shockwave): Value 500 - 650 Max 3 - 3
- Stun Grenade (Item Grenade Stun): Value 500 - 650 Max 3 - 3
- Gun (Item Gun Handgun): Value 9500 - 12000 Max 1 - 1
- Shotgun (Item Gun Shotgun): Value 18000 - 25000 Max 1 - 1
- Tranq Gun (Item Gun Tranq): Value 3500 - 4500 Max 1 - 1
- Large Health Pack (100) (Item Health Pack Large): Value 2000 - 3000 Max 3 - 3
- Medium Health Pack (50) (Item Health Pack Medium): Value 1000 - 1500 Max 3 - 3
- Small Health Pack (25) (Item Health Pack Small): Value 500 - 750 Max 3 - 3
- Baseball Bat (Item Melee Baseball Bat): Value 5500 - 7500 Max 1 - 1
- Frying Pan (Item Melee Frying Pan): Value 5500 - 7500 Max 1 - 1
- Inflatable Hammer (Item Melee Inflatable Hammer): Value 2000 - 3000 Max 1 - 1
- Sledge Hammer (Item Melee Sledge Hammer): Value 9500 - 12000 Max 1 - 1
- Sword (Item Melee Sword): Value 5500 - 7500 Max 1 - 1
- Explosive Mine (Item Mine Explosive): Value 500 - 650 Max 3 - 3
- Shockwave Mine (Item Mine Shockwave): Value 500 - 650 Max 3 - 3
- Stun Mine (Item Mine Stun): Value 500 - 650 Max 3 - 3
- Zero Gravity Orb (Item Orb Zero Gravity): Value 9500 - 12000 Max 1 - 1
- Energy Crystal (Item Power Crystal): Value 1200 - 2000 Max 6 - 4
- Rubber Duck (Item Rubber Duck): Value 3500 - 4500 Max 1 - 1
- Map Player Count Upgrade (Item Upgrade Map Player Count): Value 2000 - 3000 Max 1 - 1
- Stamina Upgrade (Item Upgrade Player Energy): Value 300 - 450 Max 10 - 4
- Extra Jump Upgrade (Item Upgrade Player Extra Jump): Value 2000 - 3000 Max 10 - 1
- Range Upgrade (Item Upgrade Player Grab Range): Value 1200 - 2000 Max 10 - 3
- Strength Upgrade (Item Upgrade Player Grab Strength): Value 1200 - 2000 Max 10 - 2
- Health Upgrade (Item Upgrade Player Health): Value 850 - 1100 Max 10 - 2
- Sprint Speed Upgrade (Item Upgrade Player Sprint Speed): Value 1200 - 2000 Max 10 - 3
- Tumble Launch Upgrade (Item Upgrade Player Tumble Launch): Value 850 - 1100 Max 10 - 1
- Valuable Tracker (Item Valuable Tracker): Value 3500 - 4500 Max 1 - 1

## Enemy Structures

### Trudge

- Enemy - Slow Walker(Clone) - UnityEngine.Transform, EnemyChecklist, Photon.Pun.PhotonView, EnemyParent
   |- Particles - UnityEngine.Transform
   |   |- Death Particles - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint
   |   |   |- Death Impact - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Death Bits Far - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Death Bits Short - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Death Smoke - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |- Enable - UnityEngine.Transform
   |   |- Controller - UnityEngine.Transform, Photon.Pun.PhotonView, EnemySlowWalker, Enemy, EnemyHealth, EnemyStateSpawn, EnemyStateDespawn, EnemyStateStunned, EnemyVision, EnemyStateInvestigate, UnityEngine.AI.NavMeshAgent, EnemyNavMeshAgent, EnemyOnScreen, EnemyPlayerDistance, EnemyPlayerRoom, Ardot.REPO.REPOverhaul.TrudgeOverride
   |   |   |- Attack Offset - UnityEngine.Transform
   |   |   |   |- Follow - UnityEngine.Transform
   |   |- [VISUALS] - UnityEngine.Transform, EnemySlowWalkerAnim, UnityEngine.Animations.ParentConstraint, UnityEngine.Animator
   |   |   |- [ANIM S&S BOT] - UnityEngine.Transform
   |   |   |   |- [ANIM S&S MID] - UnityEngine.Transform
   |   |   |   |   |- [ANIM BODY PIVOT BOT] - UnityEngine.Transform
   |   |   |   |   |   |- [BODY BOT] - UnityEngine.Transform
   |   |   |   |   |   |   |- ANIM body_bot - UnityEngine.Transform
   |   |   |   |   |   |   |   |- ANIM body_bot FREE - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- mesh body_bot - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- [FLESH BACK] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- ANIM flesh_back - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- mesh flesh_back - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- [EYE] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- ANIM eye - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- ANIM eye_wiggle - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- mesh eye - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |- [BODY TOP] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- ANIM body_top - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- ANIM body_top FREE - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- mesh body_top - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- [NECK] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- [NECK BASE] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |- ANIM neck_base - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |- mesh neck_base - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |   |   |- LOOK AT - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- code_LookAt - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- Hurt Collider - LookUnderAttack - UnityEngine.Transform, UnityEngine.BoxCollider, HurtCollider
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- [NECK 01] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- ANIM neck_01 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- code_neck_01_target - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- code_neck_01_source - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- mesh neck_01 - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, UnityEngine.BoxCollider
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- [NECK 02] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- ANIM neck_02 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- code_neck_02_target - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- code_neck_02_source - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- mesh neck_02 - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, UnityEngine.BoxCollider
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- [NECK 03] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- ANIM neck_03 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- code_neck_03_target - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- code_neck_03_source - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- mesh neck_03 - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, UnityEngine.BoxCollider
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- [HEAD] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- ANIM head - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- [HEAD TOP] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- ANIM head_top - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- mesh head_top - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, UnityEngine.BoxCollider
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- [HEAD EYE FLESH] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- ANIM head_eye_flesh - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- code_head_eye_flesh_target - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- code_head_eye_flesh_source - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- mesh head_eye_flesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- [HEAD EYE] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- ANIM head_eye - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- code_head_eye_target - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- code_head_eye_source - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- mesh head_eye - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- [HEAD BOT] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- ANIM head_bot - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- mesh head_bot - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, UnityEngine.BoxCollider
   |   |   |   |   |   |   |   |   |   |- [ARM L] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- ANIM arm_L - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- mesh arm_L - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, UnityEngine.BoxCollider
   |   |   |   |   |   |   |   |   |   |   |   |- [SHOULDER L] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |- ANIM shoulder_L - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |- mesh shoulder_L - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |   |   |- [FLESH SHOULDER L] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- ANIM flesh_shoulder_L - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |   |- mesh flesh_shoulder_L - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |- [ARM R] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- ANIM arm_R - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- mesh arm_R - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, UnityEngine.BoxCollider
   |   |   |   |   |   |   |   |   |   |- [SHOULDER R] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- ANIM shoulder_R - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- mesh shoulder_R - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |- [FLESH SHOULDER R] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |- ANIM flesh_shoulder_R - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |   |- mesh flesh_shoulder_R - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Audio Source - Mace Trailing - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Slow Walker Effect Sparks - UnityEngine.Transform, SlowWalkerSparkEffect
   |   |   |   |   |   |   |   |   |   |   |   |- Particle Ding Light - UnityEngine.Transform, UnityEngine.Light
   |   |   |   |   |   |   |   |   |   |   |   |- Particles Sparks - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |   |   |   |   |   |   |   |   |   |- Smoke - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |   |   |   |   |   |   |   |   |   |- Particle Ding Red - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |   |   |   |   |   |- [FLAP FRONT] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- ANIM flap_front - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- code_flap_front_target - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- code_flap_front_source - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- mesh flap_front - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |- [FLAP L] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- ANIM flap_L - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- code_flap_L_target - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- code_flap_L_source - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- mesh flap_L - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |- [FLAP R] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- ANIM flap_R - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- code_flap_R_target - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- code_flap_R_source - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- mesh flap_R - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |- [FLAP BACK] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- ANIM flap_back - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- code_flap_back_target - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- code_flap_back_source - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- mesh flap_back - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Slow Walker - Leg L - UnityEngine.Transform
   |   |   |   |   |   |- ANIM foot_bot_L - UnityEngine.Transform
   |   |   |   |   |   |   |- [FOOT TOP_L] - UnityEngine.Transform
   |   |   |   |   |   |   |   |- ANIM foot_top_L - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- mesh foot_top_L - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |- [LEG 01_L] - UnityEngine.Transform
   |   |   |   |   |   |   |   |- ANIM leg_01_L - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- mesh leg_01_L - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- [LEG 02_L] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- ANIM leg_02_L - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- mesh leg_02_L - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Slow Walker - Leg R - UnityEngine.Transform
   |   |   |   |   |   |- ANIM foot_bot_R - UnityEngine.Transform
   |   |   |   |   |   |   |- [FOOT TOP_R] - UnityEngine.Transform
   |   |   |   |   |   |   |   |- ANIM foot_top_R - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- mesh foot_top_R - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |- [LEG 01_R] - UnityEngine.Transform
   |   |   |   |   |   |   |   |- ANIM leg_01_R - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- mesh leg_01_R - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- [LEG 02_R] - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- ANIM leg_02_R - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- mesh leg_02_R - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |- Club Hit Point - UnityEngine.Transform
   |   |   |- Vision Transform - UnityEngine.Transform
   |   |   |- Particle Constraint - UnityEngine.Transform
   |   |   |- On Screen Point Top - UnityEngine.Transform
   |   |   |- On Screen Point Mid - UnityEngine.Transform
   |   |   |- On Screen Point Bot - UnityEngine.Transform
   |   |   |- Feet Transform - UnityEngine.Transform
   |   |   |- Kill Look At Transform - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint
   |   |   |- Hurt Collider - StuckAttack - UnityEngine.Transform, UnityEngine.BoxCollider, HurtCollider
   |   |- [AUDIO] - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint
   |   |   |- Audio Source - Stunned - Loop - UnityEngine.Transform, UnityEngine.AudioSource, UnityEngine.AudioLowPassFilter, AudioLowPassLogic
   |   |- Rigidbody - UnityEngine.Transform, Photon.Pun.PhotonView, Photon.Pun.PhotonTransformView, EnemyRigidbody, PhysGrabObject, NotValuableObject, RoomVolumeCheck, PhysGrabObjectImpactDetector, UnityEngine.Rigidbody, EnemyGrounded, EnemyJump
   |   |   |- Collider - UnityEngine.Transform, UnityEngine.CapsuleCollider, PhysGrabObjectCapsuleCollider, PhysGrabObjectCollider
   |   |   |- Player Collision - UnityEngine.Transform, UnityEngine.CapsuleCollider, PhysGrabObjectCollider
   |   |   |- Center - UnityEngine.Transform
   |   |   |- Grounded - UnityEngine.Transform, UnityEngine.BoxCollider
   |   |- Audio Stun - UnityEngine.Transform, UnityEngine.AudioSource, UnityEngine.AudioLowPassFilter, AudioLowPassLogic, UnityEngine.Animations.ParentConstraint
   |- Slow Walker Attack - UnityEngine.Transform, Photon.Pun.PhotonView, SlowWalkerAttack
   |   |- Particle Ding Light - UnityEngine.Transform, UnityEngine.Light
   |   |- Buildup - UnityEngine.Transform
   |   |   |- Hurt Collider - Attack - UnityEngine.Transform, UnityEngine.BoxCollider, HurtCollider
   |   |   |- VacuumSphere - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, UnityEngine.SphereCollider
   |   |   |- Hurt Collider - UnityEngine.Transform, HurtCollider, UnityEngine.SphereCollider
   |   |   |- Particle Ding - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particle Ding Red - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particle Cracks Buildup - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particle Cracks Suck In - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particle Shockwave - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Suck Particles - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Metal Bits - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Suck Particles Buildup - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |- Impact - UnityEngine.Transform
   |   |   |- Hurt Colliders - UnityEngine.Transform
   |   |   |   |- Hurt Collider - UnityEngine.Transform, HurtCollider, UnityEngine.SphereCollider
   |   |   |- Particle Shockwave 2 - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Smoke - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Smoke (1) - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particles Poof Up - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particle Cracks - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particles Poof - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particle Ding Red - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particle Ding White - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Metal Bits Up - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Metal Bits - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |- Slow Walker Jump - UnityEngine.Transform, SlowWalkerJumpEffect, UnityEngine.Animations.ParentConstraint
   |   |- Particle Ding Light - UnityEngine.Transform, UnityEngine.Light
   |   |- Rotation - UnityEngine.Transform
   |   |   |- Particle Shockwave 2 - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particles Poof (1) - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particle Ding Red - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particles Poof - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particle Cracks Buildup - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particle Cracks Buildup (1) - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Smoke - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Smoke (2) - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Smoke (1) - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer

### Gnome

- Enemy - Gnome - UnityEngine.Transform, EnemyChecklist, Photon.Pun.PhotonView, EnemyParent
   |- Enable - UnityEngine.Transform
   |   |- Plane - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, UnityEngine.MeshCollider
   |   |- [VISUALS] - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, EnemyGnomeAnim, UnityEngine.Animator
   |   |   |- Hurt Collider - UnityEngine.Transform, UnityEngine.BoxCollider, HurtCollider
   |   |   |- ____________________________ - UnityEngine.Transform
   |   |   |   |- ANIM BOT - UnityEngine.Transform
   |   |   |   |   |- ____________________________ - UnityEngine.Transform
   |   |   |   |   |   |- ANIM MID - UnityEngine.Transform
   |   |   |   |   |   |   |- ____________________________ - UnityEngine.Transform
   |   |   |   |   |   |   |   |- ANIM HEAD - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- mesh_head - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |- ____________________________ - UnityEngine.Transform
   |   |   |   |   |   |   |   |- ANIM ARM RIGHT - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- mesh_arm - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |- ANIM ARM SWING - UnityEngine.Transform
   |   |   |   |   |   |   |   |- ____________________________ - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- ANIM ARM LEFT - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- ____________________________ - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- ANIM PICKAXE - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- mesh_pickaxe - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- mesh_arm - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |- mesh_body - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |- Rigidbody - UnityEngine.Transform, Photon.Pun.PhotonView, Photon.Pun.PhotonTransformView, EnemyRigidbody, PhysGrabObject, NotValuableObject, RoomVolumeCheck, PhysGrabObjectImpactDetector, UnityEngine.Rigidbody, EnemyGrounded, EnemyJump
   |   |   |- Grounded - UnityEngine.Transform, UnityEngine.BoxCollider
   |   |   |- Avoid Collider - UnityEngine.Transform, UnityEngine.BoxCollider
   |   |   |- Vision - UnityEngine.Transform
   |   |   |- Kill Look At - UnityEngine.Transform
   |   |   |- Center - UnityEngine.Transform
   |   |   |- Semi Capsule Collider - UnityEngine.Transform, UnityEngine.CapsuleCollider, PhysGrabObjectCapsuleCollider, PhysGrabObjectCollider
   |   |- Controller - UnityEngine.Transform, EnemyGnome, Photon.Pun.PhotonView, Enemy, EnemyHealth, EnemyStateSpawn, EnemyStateDespawn, EnemyStateStunned, UnityEngine.AI.NavMeshAgent, EnemyNavMeshAgent, EnemyStateInvestigate, EnemyVision
   |   |   |- Move Offset - UnityEngine.Transform
   |   |   |   |- Back Away Offset - UnityEngine.Transform
   |   |   |   |   |- Rotation - UnityEngine.Transform
   |   |   |   |   |   |- Rigidbody Follow - UnityEngine.Transform
   |- Stun Fly Loop - UnityEngine.Transform, EnemyGnomeStunFly, UnityEngine.Animations.ParentConstraint, UnityEngine.AudioSource, UnityEngine.AudioLowPassFilter, AudioLowPassLogic
   |- Other - UnityEngine.Transform
   |   |- Death Effect - UnityEngine.Transform, UnityEngine.Animations.PositionConstraint
   |   |   |- Particle Bits Short - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particle Bits Far - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Particle Smoke - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer

### Headman

- Enemy - Head - UnityEngine.Transform, EnemyChecklist, Photon.Pun.PhotonView, EnemyParent
   |- Enable - UnityEngine.Transform
   |   |- Look At Transform - UnityEngine.Transform, UnityEngine.Animations.PositionConstraint
   |   |- Mesh - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint
   |   |   |- Animation System - UnityEngine.Transform, UnityEngine.Animator, EnemyHeadAnimationSystem, UnityEngine.AudioSource, Photon.Pun.PhotonView, UnityEngine.AudioLowPassFilter, AudioLowPassLogic
   |   |   |   |- Main Animation - UnityEngine.Transform
   |   |   |   |   |- Particle Positions - UnityEngine.Transform
   |   |   |   |   |   |- Bot - UnityEngine.Transform
   |   |   |   |   |   |- Top - UnityEngine.Transform
   |   |   |   |   |- Bot Mesh - UnityEngine.Transform
   |   |   |   |   |   |- Offset - UnityEngine.Transform
   |   |   |   |   |   |   |- Bot Animation - UnityEngine.Transform
   |   |   |   |   |   |   |   |- Mesh - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- Enemy Head Bot - Bite - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Gum Top - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Gum Top Anim - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- Gum Top Mesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Gum Bot - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Gum Bot Anim - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- Gum Bot Mesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Bot Anim - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Bot Mesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- Enemy Head Bot - Closed - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Head Bot_Mouth Closed - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Teeth Top_Mouth Closed - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Teeth Bot_Mouth Closed - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Gum Top_Mouth Closed - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Gum Bot_Mouth Closed - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Mouth Inside.003 - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Offset - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- Enemy Head Bot - Half Open - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Headman_Mouth Half Open 03 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Teeth Top_Mouth Half Open - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Teeth Bot_Mouth Half Open - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Head Bot_Mouth Half Open - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |- Mouth Inside.002 - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Gum Top_Mouth Closed.001 - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Gum Bot_Mouth Closed.001 - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Offset - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- Enemy Head Bot - Chase - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Offset - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Tilt - UnityEngine.Transform, EnemyHeadBotTilt
   |   |   |   |   |   |   |   |   |   |   |   |- Head Bot - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |- Mouth Inside  - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |- Teeth Top_Chase - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |- Teeth Bot_Chase - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |- Gum Top_Chase - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |- Gum Bot_Chase - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- Enemy Head Bot - Idle - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Mouth Inside (plane) - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Head Bot - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Top Mesh - UnityEngine.Transform
   |   |   |   |   |   |- Offset - UnityEngine.Transform
   |   |   |   |   |   |   |- Top Animation - UnityEngine.Transform
   |   |   |   |   |   |   |   |- Mesh - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- Enemy Head Top - Chase - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- hair lump_static - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Eye Sockets - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Ears - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Headman_Mouth Half Open - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Head Top_Mouth Half Open - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Hair Static_Mouth Half Open - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Eye Right - UnityEngine.Transform, EnemyHeadEye
   |   |   |   |   |   |   |   |   |   |   |- Eye R Tremble - UnityEngine.Transform, EnemyHeadEyeTremble
   |   |   |   |   |   |   |   |   |   |   |   |- Eye Pupil R Mesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, EnemyHeadPupil
   |   |   |   |   |   |   |   |   |   |   |   |- Eye R Mesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Eye Left - UnityEngine.Transform, EnemyHeadEye
   |   |   |   |   |   |   |   |   |   |   |- Eye L Tremble - UnityEngine.Transform, EnemyHeadEyeTremble
   |   |   |   |   |   |   |   |   |   |   |   |- Eye Pupil L Mesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, EnemyHeadPupil
   |   |   |   |   |   |   |   |   |   |   |   |- Eye L Mesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Hair Static - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- Enemy Head Top - Idle - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- hair lump_static - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Eye Sockets - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Ears - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Hair Static - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Head Top - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Eye Right - UnityEngine.Transform, EnemyHeadEye
   |   |   |   |   |   |   |   |   |   |   |- Eye R Tremble - UnityEngine.Transform, EnemyHeadEyeTremble
   |   |   |   |   |   |   |   |   |   |   |   |- Eye Pupil R Mesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, EnemyHeadPupil
   |   |   |   |   |   |   |   |   |   |   |   |- Eye R Mesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |- Eye Left - UnityEngine.Transform, EnemyHeadEye
   |   |   |   |   |   |   |   |   |   |   |- Eye L Tremble - UnityEngine.Transform, EnemyHeadEyeTremble
   |   |   |   |   |   |   |   |   |   |   |   |- Eye Pupil L Mesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, EnemyHeadPupil
   |   |   |   |   |   |   |   |   |   |   |   |- Eye L Mesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |- Hair Targets - UnityEngine.Transform
   |   |   |   |- Logic - UnityEngine.Transform
   |   |   |   |   |- Chase Loop 2 - UnityEngine.Transform, UnityEngine.AudioSource, UnityEngine.AudioLowPassFilter, AudioLowPassLogic
   |   |   |   |   |- Hurt Box - UnityEngine.Transform, UnityEngine.BoxCollider, HurtCollider
   |   |   |   |   |- Audio Loop - UnityEngine.Transform, EnemyHeadLoop, UnityEngine.AudioSource, UnityEngine.AudioLowPassFilter, AudioLowPassLogic
   |   |   |   |   |- Kill Look At - UnityEngine.Transform
   |   |   |   |   |- Vision - UnityEngine.Transform
   |   |- Hair - UnityEngine.Transform
   |   |   |- Hair 03 - UnityEngine.Transform
   |   |   |   |- Mesh - UnityEngine.Transform, UnityEngine.SkinnedMeshRenderer
   |   |   |   |- Rig - UnityEngine.Transform
   |   |   |   |   |- Top - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.Animations.ScaleConstraint
   |   |   |   |   |- Mid - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.Animations.ScaleConstraint
   |   |   |   |   |- Root - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.Animations.ScaleConstraint
   |   |   |   |- Follow - UnityEngine.Transform
   |   |   |   |   |- Top - UnityEngine.Transform, EnemyHeadHair
   |   |   |   |   |   |- Debug - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Mid - UnityEngine.Transform, EnemyHeadHair
   |   |   |   |   |   |- Top Target - UnityEngine.Transform
   |   |   |   |   |   |- Debug - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Root - UnityEngine.Transform, EnemyHeadHair
   |   |   |   |   |   |- Mid Target - UnityEngine.Transform
   |   |   |   |   |   |- Debug - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |- Target - UnityEngine.Transform, EnemyHeadHairTarget
   |   |   |   |   |- Lean - UnityEngine.Transform, EnemyHeadHairLean
   |   |   |   |   |   |- Tilt - UnityEngine.Transform, EnemyHeadHairTilt
   |   |   |- Hair 02 - UnityEngine.Transform
   |   |   |   |- Mesh - UnityEngine.Transform, UnityEngine.SkinnedMeshRenderer
   |   |   |   |- Rig - UnityEngine.Transform
   |   |   |   |   |- Top - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.Animations.ScaleConstraint
   |   |   |   |   |- Mid - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.Animations.ScaleConstraint
   |   |   |   |   |- Root - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.Animations.ScaleConstraint
   |   |   |   |- Follow - UnityEngine.Transform
   |   |   |   |   |- Top - UnityEngine.Transform, EnemyHeadHair
   |   |   |   |   |   |- Debug - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Mid - UnityEngine.Transform, EnemyHeadHair
   |   |   |   |   |   |- Top Target - UnityEngine.Transform
   |   |   |   |   |   |- Debug - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Root - UnityEngine.Transform, EnemyHeadHair
   |   |   |   |   |   |- Mid Target - UnityEngine.Transform
   |   |   |   |   |   |- Debug - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |- Target - UnityEngine.Transform, EnemyHeadHairTarget
   |   |   |   |   |- Lean - UnityEngine.Transform, EnemyHeadHairLean
   |   |   |   |   |   |- Tilt - UnityEngine.Transform, EnemyHeadHairTilt
   |   |   |- Hair 01 - UnityEngine.Transform
   |   |   |   |- Mesh - UnityEngine.Transform, UnityEngine.SkinnedMeshRenderer
   |   |   |   |- Rig - UnityEngine.Transform
   |   |   |   |   |- Top - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.Animations.ScaleConstraint
   |   |   |   |   |- Mid - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.Animations.ScaleConstraint
   |   |   |   |   |- Root - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.Animations.ScaleConstraint
   |   |   |   |- Follow - UnityEngine.Transform
   |   |   |   |   |- Top - UnityEngine.Transform, EnemyHeadHair
   |   |   |   |   |   |- Debug - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Mid - UnityEngine.Transform, EnemyHeadHair
   |   |   |   |   |   |- Top Target - UnityEngine.Transform
   |   |   |   |   |   |- Debug - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Root - UnityEngine.Transform, EnemyHeadHair
   |   |   |   |   |   |- Mid Target - UnityEngine.Transform
   |   |   |   |   |   |- Debug - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |- Target - UnityEngine.Transform, EnemyHeadHairTarget
   |   |   |   |   |- Lean - UnityEngine.Transform, EnemyHeadHairLean
   |   |   |   |   |   |- Tilt - UnityEngine.Transform, EnemyHeadHairTilt
   |   |- Attack Trigger - UnityEngine.Transform, UnityEngine.Rigidbody, UnityEngine.BoxCollider, RigidbodyFollow, EnemyTriggerAttack
   |   |- Eye Target - UnityEngine.Transform, EnemyHeadEyeTarget
   |   |   |- Idle - UnityEngine.Transform, EnemyHeadEyeIdle
   |   |   |   |- Target Left - UnityEngine.Transform
   |   |   |   |   |- Debug - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |- Target Right - UnityEngine.Transform
   |   |   |   |   |- Debug - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |- Rigidbody - UnityEngine.Transform, Photon.Pun.PhotonView, Photon.Pun.PhotonTransformView, EnemyRigidbody, PhysGrabObject, NotValuableObject, RoomVolumeCheck, PhysGrabObjectImpactDetector, UnityEngine.Rigidbody
   |   |   |- Collision Bot - UnityEngine.Transform, UnityEngine.CapsuleCollider, PhysGrabObjectCollider
   |   |   |- Collision Top - UnityEngine.Transform, UnityEngine.CapsuleCollider, PhysGrabObjectCollider
   |   |- Controller - UnityEngine.Transform, EnemyHeadController, Photon.Pun.PhotonView, Enemy, EnemyHealth, EnemyStateSpawn, EnemyStateDespawn, EnemyStateStunned, EnemyVision, EnemyPlayerDistance, EnemyOnScreen, EnemyPlayerRoom, UnityEngine.AI.NavMeshAgent, EnemyNavMeshAgent, EnemyAttackStuckPhysObject, EnemyStateRoaming, EnemyStateInvestigate, EnemyStateChaseBegin, EnemyStateSneak, EnemyStateChase, EnemyStateChaseSlow, EnemyStateChaseEnd, EnemyStateLookUnder
   |   |   |- Visual - UnityEngine.Transform, Photon.Pun.PhotonView, EnemyHeadVisual
   |   |   |   |- Up - UnityEngine.Transform, EnemyHeadUp
   |   |   |   |   |- Rotation - UnityEngine.Transform
   |   |   |   |   |   |- Look Under - UnityEngine.Transform, AnimatedOffset
   |   |   |   |   |   |   |- Float - UnityEngine.Transform, EnemyHeadFloat
   |   |   |   |   |   |   |   |- Noise - UnityEngine.Transform, AnimNoise
   |   |   |   |   |   |   |   |   |- Chase Offset - UnityEngine.Transform, EnemyHeadChaseOffset
   |   |   |   |   |   |   |   |   |   |- Tilt - UnityEngine.Transform, EnemyHeadTilt
   |   |   |   |   |   |   |   |   |   |   |- Lean - UnityEngine.Transform, EnemyHeadLean
   |   |   |   |   |   |   |   |   |   |   |   |- Follow Target - UnityEngine.Transform
   |- Particles - UnityEngine.Transform
   |   |- Death Smoke - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer, UnityEngine.Animations.ParentConstraint
   |   |- Death Bits Short - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |- Death Bits Far - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |- Death Impact - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |- Death Eyes - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |- Teleport Bot - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |- Teleport Top - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer

### Robe

- Enemy - Robe - UnityEngine.Transform, EnemyChecklist, Photon.Pun.PhotonView, EnemyParent
   |- Particles - UnityEngine.Transform, EnemyRobePersistent
   |   |- Constant Smoke - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |- Teleport - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |- Death Particles - UnityEngine.Transform, UnityEngine.Animations.ParentConstraint
   |   |   |- Death Mask Top - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Death Mask Bot - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Death Impact - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Death Bits Far - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Death Bits Short - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |   |   |- Death Smoke - UnityEngine.Transform, UnityEngine.ParticleSystem, UnityEngine.ParticleSystemRenderer
   |- Enable - UnityEngine.Transform
   |   |- Controller - UnityEngine.Transform, Photon.Pun.PhotonView, EnemyRobe, Enemy, EnemyHealth, EnemyStateSpawn, EnemyStateDespawn, EnemyStateStunned, EnemyVision, EnemyStateInvestigate, UnityEngine.AI.NavMeshAgent, EnemyNavMeshAgent, EnemyOnScreen, EnemyPlayerDistance, EnemyPlayerRoom
   |   |   |- Follow - UnityEngine.Transform
   |   |- Visual - UnityEngine.Transform, EnemyRobeAnim, UnityEngine.Animations.ParentConstraint, UnityEngine.Animator
   |   |   |- Main Animation - UnityEngine.Transform
   |   |   |   |- Robe Hand - UnityEngine.Transform
   |   |   |   |   |- _____________________________A - UnityEngine.Transform
   |   |   |   |   |   |- Arm - UnityEngine.Transform
   |   |   |   |   |   |   |- Robe Hand Base - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |- Hand Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |- _____________________________H - UnityEngine.Transform
   |   |   |   |   |   |   |   |- Hand - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- Robe Hand Base - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- Hand Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- ______________________________T - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Thumb Bot - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Jitter - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Thumb Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger S - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- ______________________________T2 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- Thumb Top - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger E - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |   |- Thumb Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- ______________________________F3 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Finger 3 Bot - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger S - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Finger 3 Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- ______________________________F3_2 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- Finger 3 Top - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger E - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |   |- Finger 3 Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- ______________________________F2 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Finger 2 Bot - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger S - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Finger 2 Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- ______________________________F2_2 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- Finger 2 Top - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger E - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |   |- Finger 2 Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- ______________________________F1 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Finger 1 Bot - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger S - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Finger 1 Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- ______________________________F1_2 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- Finger 1 Top - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger E - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |   |- Finger 1 Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |- Robe Hand (1) - UnityEngine.Transform
   |   |   |   |   |- _____________________________A - UnityEngine.Transform
   |   |   |   |   |   |- Arm - UnityEngine.Transform
   |   |   |   |   |   |   |- Robe Hand Base - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |- Hand Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |- _____________________________H - UnityEngine.Transform
   |   |   |   |   |   |   |   |- Hand - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- Robe Hand Base - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- Hand Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- ______________________________T - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Thumb Bot - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Jitter - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Thumb Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger S - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- ______________________________T2 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- Thumb Top - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger E - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |   |- Thumb Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- ______________________________F3 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Finger 3 Bot - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger S - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Finger 3 Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- ______________________________F3_2 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- Finger 3 Top - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger E - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |   |- Finger 3 Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- ______________________________F2 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Finger 2 Bot - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger S - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Finger 2 Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- ______________________________F2_2 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- Finger 2 Top - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger E - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |   |- Finger 2 Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |- ______________________________F1 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |- Finger 1 Bot - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger S - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- Finger 1 Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |- ______________________________F1_2 - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |- Finger 1 Top - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |   |   |   |   |- Robe Hand Finger E - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |   |   |   |   |   |   |- Finger 1 Bot Greybox - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |- Robe Hand Position (1) - UnityEngine.Transform
   |   |   |   |- POSES________________________________________ - UnityEngine.Transform
   |   |   |   |   |- Body Closed - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Body Open - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Body Attack - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Head - UnityEngine.Transform
   |   |   |   |   |   |- Vision Origin - UnityEngine.Transform
   |   |   |   |   |   |- Head Shake - UnityEngine.Transform
   |   |   |   |   |   |   |- Jaw - Closed - UnityEngine.Transform
   |   |   |   |   |   |   |   |- mesh_jaw closed - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |- Jaw - Open - UnityEngine.Transform
   |   |   |   |   |   |   |   |- Jaw Shake - Open - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- mesh_jaw open - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |- Jaw - Attack - UnityEngine.Transform
   |   |   |   |   |   |   |   |- Jaw Shake - Attack - UnityEngine.Transform
   |   |   |   |   |   |   |   |   |- mesh_jaw attack - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |   |   |- Head Top - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- End Piece - UnityEngine.Transform
   |   |   |   |   |   |- Target - UnityEngine.Transform
   |   |   |   |   |   |- Source - UnityEngine.Transform
   |   |   |   |   |   |   |- Mesh - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer
   |   |   |   |   |- Audio Hand Idle - UnityEngine.Transform, UnityEngine.AudioSource, UnityEngine.AudioLowPassFilter, AudioLowPassLogic
   |   |   |   |   |- Audio Hand Aggressive - UnityEngine.Transform, UnityEngine.AudioSource, UnityEngine.AudioLowPassFilter, AudioLowPassLogic
   |   |   |   |- Hurt Collider - UnityEngine.Transform, UnityEngine.BoxCollider, HurtCollider
   |   |   |- Particle Constraint - UnityEngine.Transform
   |   |   |- Spot Light - UnityEngine.Transform, UnityEngine.Light
   |   |   |- On Screen Point Top - UnityEngine.Transform
   |   |   |- On Screen Point Mid - UnityEngine.Transform
   |   |   |- On Screen Point Bot - UnityEngine.Transform
   |   |- Rigidbody - UnityEngine.Transform, Photon.Pun.PhotonView, Photon.Pun.PhotonTransformView, EnemyRigidbody, PhysGrabObject, NotValuableObject, RoomVolumeCheck, PhysGrabObjectImpactDetector, UnityEngine.Rigidbody
   |   |   |- Collider - UnityEngine.Transform, UnityEngine.CapsuleCollider, PhysGrabObjectCapsuleCollider, PhysGrabObjectCollider
   |   |   |- Center - UnityEngine.Transform
   |   |- Audio Target Player - UnityEngine.Transform, UnityEngine.AudioSource, UnityEngine.AudioLowPassFilter, AudioLowPassLogic, UnityEngine.Animations.ParentConstraint
   |   |- Audio Stun - UnityEngine.Transform, UnityEngine.AudioSource, UnityEngine.AudioLowPassFilter, AudioLowPassLogic, UnityEngine.Animations.ParentConstraint
   |- DEBUG PLANE - UnityEngine.Transform, UnityEngine.MeshFilter, UnityEngine.MeshRenderer, UnityEngine.MeshCollider, DisableInGame