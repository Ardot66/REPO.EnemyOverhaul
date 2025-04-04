# Notes

## Misc Changes

- Make pits fall into adjacent rooms from the ceiling, dealing non-lethal fall damage.
- Continue tweaking shop-item balance
- Come up with a better recharge / energy system for tools?
  - Special items that are large and breakable (might have no value either), but provide power crystals when extracted?
- Improve health economy, similar idea to energy crystal, specialised items?
- Make melee weapons have no charge, more skill based somehow?
  - Maybe a durability system where new weapons have to be purchased after old ones break?
- Make the minimap an upgrade, potentially remove entirely.

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
  - Needs big changes
- Banger
- Headman
  - Change to aggro players only when holding items above a certain value?
- Robe
  - It wants to be part of the team, gets angry and enters rage if players try to run away from it?
    - Might also aggro if bumped by items held by player
- Huntsman (Fixed!)
- Reaper
  - Only aggro players not moving?
- Clown (Decent already)
- Trudge
  - Really hates extraction points, will approach and destroy them.
    - An indicator that a trudge is approaching an extraction point? (e.g. Unidentified object approaching extraction, distance 10m)
    - Approaches the ship in the same way if no extractions are opened, preventing cheese where players stack stuff near unopened extractions.
    - When an extraction is destroyed, the amount needed is added to the next one?
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