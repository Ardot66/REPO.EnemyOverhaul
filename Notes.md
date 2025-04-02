# Notes

## Enemies

- Peeper
- Shadow-child
- Gnome
- Apex-predator
- Spewer
- Rugrat
- Animal
- Upscream
- Hidden
- Chef
- Bowtie
- Mentalist
- Banger
- Headman
- Robe
- Huntsman
- Reaper
- Clown
- Trudge

## Items

- C.A.R.T.: 12000
- POCKET C.A.R.T.: 4500
- Recharge Drone: 1100
- Feather Drone: 4500
- Indestructible Drone: 7500
- Roll Drone: 3000
- Zero Gravity Drone: 7500
- Extraction Tracker: 2000
- Duct Taped Grenades: 450
- Grenade: 650
- Human Grenade: 500
- Shockwave Grenade: 650
- Stun Grenade: 650
- Gun: 12000
- Shotgun: 25000
- Tranq Gun: 4500
- Large Health Pack (100): 3000
- Medium Health Pack (50): 1500
- Small Health Pack (25): 750
- Baseball Bat: 7500
- Frying Pan: 7500
- Inflatable Hammer: 3000
- Sledge Hammer: 12000
- Sword: 7500
- Explosive Mine: 650
- Shockwave Mine: 650
- Stun Mine: 650
- Zero Gravity Orb: 12000
- Energy Crystal: 2000
- Rubber Duck: 4500
- Map Player Count Upgrade: 3000
- Stamina Upgrade: 450
- Extra Jump Upgrade: 3000
- Range Upgrade: 2000
- Strength Upgrade: 2000
- Health Upgrade: 1100
- Sprint Speed Upgrade: 2000
- Tumble Launch Upgrade: 1100
- Valuable Tracker: 4500

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