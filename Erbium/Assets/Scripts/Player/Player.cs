﻿using System.Collections.Generic;
using Animators;
using Characters;
using Characters.Armour;
using Characters.Attack;
using Characters.Damage;
using Characters.Health;
using Characters.Hurtbox;
using Characters.Movement;
using General.Util;
using Player.MovementDirection;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Stats))]
    public class Player : MonoBehaviour, IPlayer, IDamageDealer {
        private static readonly FastEnumIntEqualityComparer<MovementEnum> FastEnumIntEqualityComparer =
            new FastEnumIntEqualityComparer<MovementEnum>();

        private readonly Dictionary<MovementEnum, IMovement> movements =
            new Dictionary<MovementEnum, IMovement>(FastEnumIntEqualityComparer);

        private IMovement movement;
        private IAnimatorFacade animatorFacade;
        private IMovementDirection movementDirection;
        private IHealthComponent healthComponent;
        private IArmour armour;
        private IAttackManager attackManager;
        private Rigidbody rbd;
        private Stats stats;
        public GameObject stateGo;
        public StateUI stateUi;
        [SerializeField] private CameraView cameraView;
        [SerializeField] private ComboUI comboUi;


        private void Start() {
            rbd = GetComponent<Rigidbody>();
            stats = GetComponent<Stats>();
            movementDirection = setCameraDirection(cameraView);
            animatorFacade = new AnimatorFacade(GetComponentInChildren<ICharacterAnimator>(), this);
            initMovements();
            movement = movements[MovementEnum.Ground];
            healthComponent = new HealthComponent(this);
            armour = new Armour(this);
            attackManager = new AttackManager(animatorFacade, this, comboUi);
            stateUi = stateGo.GetComponent<StateUI>();
        }

        private void FixedUpdate() {
            movement.move(movementDirection.getDirection());
            Debug.Log(rbd.velocity.magnitude);
        }


        public void die() {
        }


        private IMovementDirection setCameraDirection(CameraView cameraView) {
            this.cameraView = cameraView;
            switch (cameraView) {
                case CameraView.AlwaysForward:
                    return new ThirdPersonCameraDirection();
                default:
                    return new ThirdPersonCameraDirection();
            }
        }

        public IHealthComponent getHealthComponent() {
            return healthComponent;
        }

        public IAnimatorFacade getAnimatorFacade() {
            return animatorFacade;
        }

        public IArmour getArmour() {
            return armour;
        }

        public IAttackManager getAttackManager() {
            return attackManager;
        }

        public Rigidbody getRigidbody() {
            return rbd;
        }

        public Transform getTransform() {
            return transform;
        }

        public IMovement getMovement() {
            return movement;
        }

        public void changeMovement(MovementEnum movementEnum) {
            movement.cleanUp();
            /*switch (movementEnum) {
                case MovementEnum.Crouch:
                    movement = new CrouchingMovement(this);
                    break;
                case MovementEnum.Ground:
                    movement = new GroundMovement(this);
                    break;
                case MovementEnum.Midair:
                    movement = new MidairMovement(this);
                    break;
                case MovementEnum.Slide:
                    movement = new SlidingMovement(this);
                    break;
            }*/
            stateUi.changeColor(movementEnum);
            movement = movements[movementEnum];
            movement.setUp();
        }

        public Stats getStats() {
            return stats;
        }

        public void changeMovementDirection(IMovementDirection movementDirection) {
            this.movementDirection = movementDirection;
        }

        public void changeMovementDirection(CameraView cameraView) {
            movementDirection = setCameraDirection(cameraView);
        }

        private void initMovements() {
            movements.Add(MovementEnum.Ground, new GroundMovement(this));
            movements.Add(MovementEnum.Midair, new MidairMovement(this));
            movements.Add(MovementEnum.Crouch, new CrouchingMovement(this));
            movements.Add(MovementEnum.Slide, new SlidingMovement(this));
            movements.Add(MovementEnum.Attack, new AttackingMovement(this));
        }

        public void dealDamage(IHurtbox hurtbox) {
            attackManager.dealDamage(hurtbox);
        }
    }
}