/* -*- mode:CSharp:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ
{

    public class Player : Task
    {
        private const float SHIELD_RADIUS = 1.5f /* radius */;

        // singleton
        static Player instance_;
        public static Player Instance { get { return instance_ ?? (instance_ = new Player()); } }

        private Posture posture_apose_;
        private Posture posture_pre_throw_l_arm_;
        private Posture posture_pre_throw_r_arm_;
        private Posture posture_throw_l_arm_;
        private Posture posture_throw_r_arm_;
        private Posture posture_pre_jump_;
        private int throwing_cnt_l_;
        private int throwing_cnt_r_;
        private float jump_tame_duration_;
        private float jump_propel_remain_;
        private bool on_ground_;
        private float on_ground_time_;
        private float bullet_tame_left_;
        private float bullet_tame_right_;
        // private float hit_time_;
        // private Vector3 hit_position_;
        private bool somersault_;

        private MuscleMotion muscle_motion_;
        public RigidbodyTransform rigidbody_;
        private int collider_;
        private Vector3 look_at_;

        private Bullet left_held_bullet_;
        private Bullet right_held_bullet_;
        public Transform robotT;
        public MuscleMotion getMuscleMotion()
        { return muscle_motion_; }
        public float TameGaugeValue() { return Mathf.Clamp(jump_tame_duration_ * 2f, 0f, 1f); }
        public float getAuraValue() { return jump_tame_duration_ > 0f || !on_ground_ ? 1f : 0f; }

        public IEnumerator initialize()
        {
            base.init();

            yield return FileUtil.preparePath("apose.dat");
            posture_apose_ = JsonUtility.FromJson<Posture>(FileUtil.content);
            Debug.Assert(posture_apose_ != null);
            yield return FileUtil.preparePath("pre_throw_l_arm.dat");
            posture_pre_throw_l_arm_ = JsonUtility.FromJson<Posture>(FileUtil.content);
            Debug.Assert(posture_pre_throw_l_arm_ != null);
            yield return FileUtil.preparePath("pre_throw_r_arm.dat");
            posture_pre_throw_r_arm_ = JsonUtility.FromJson<Posture>(FileUtil.content);
            Debug.Assert(posture_pre_throw_r_arm_ != null);
            yield return FileUtil.preparePath("throw_l_arm.dat");
            posture_throw_l_arm_ = JsonUtility.FromJson<Posture>(FileUtil.content);
            Debug.Assert(posture_throw_l_arm_ != null);
            yield return FileUtil.preparePath("throw_r_arm.dat");
            posture_throw_r_arm_ = JsonUtility.FromJson<Posture>(FileUtil.content);
            Debug.Assert(posture_throw_r_arm_ != null);
            yield return FileUtil.preparePath("pre_jump.dat");
            posture_pre_jump_ = JsonUtility.FromJson<Posture>(FileUtil.content);
            Debug.Assert(posture_throw_r_arm_ != null);
            throwing_cnt_l_ = 0;
            throwing_cnt_r_ = 0;
            jump_tame_duration_ = 0f;
            jump_propel_remain_ = 0f;
            on_ground_ = true;
            on_ground_time_ = 100f;
            bullet_tame_left_ = 0f;
            bullet_tame_right_ = 0f;
            // hit_time_ = 0f;
            somersault_ = false;

            muscle_motion_ = new MuscleMotion();
            muscle_motion_.init(posture_apose_, 80f /* damper */, 1500f /* spring */);
            MuscleMotion.Node root_node = muscle_motion_.getRootNode();
            look_at_ = new Vector3(0f, 1f, 0f);
            rigidbody_.setPosition(0f, 1f, 15f);
            var rot = Quaternion.LookRotation(look_at_ - rigidbody_.transform_.position_);
            rigidbody_.setRotation(ref rot);
            rigidbody_.setDamper(10f);
            rigidbody_.setRotateDamper(50f);
            root_node.rigidbody_.setDamper(10f);
            root_node.rigidbody_.setRotateDamper(40f);

            collider_ = MyCollider.createPlayer();
            MyCollider.initSpherePlayer(collider_, ref rigidbody_.transform_.position_, SHIELD_RADIUS /* radius */);

            // muscle_motion_.fix(MuscleMotion.Parts.Ribs, 0.4f /* interpolate_ratio */);
            // muscle_motion_.fix(MuscleMotion.Parts.Ribs2);
            // muscle_motion_.fix(MuscleMotion.Parts.Ribs3, 0.4f /* interpolate_ratio */);
            // muscle_motion_.fix(MuscleMotion.Parts.Hip, 0.1f /* interpolate_ratio */);
            {   /* body */
                float damper = 60f;
                float spring_ratio = 800f;
                muscle_motion_.setParams(MuscleMotion.Parts.Ribs, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.Ribs2, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.Ribs3, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.Hip, damper, spring_ratio);
            }
            {   /* arms */
                float damper = 50f;
                float spring_ratio = 1000f;
                muscle_motion_.setParams(MuscleMotion.Parts.L_Shoulder, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.L_UpperArm, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.L_ForeArm, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.L_Wrist, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_Shoulder, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_UpperArm, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_ForeArm, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_Wrist, damper, spring_ratio);
            }
            {   /* legs */
                float damper = 30f;
                float spring_ratio = 500f;
                muscle_motion_.setParams(MuscleMotion.Parts.L_Thigh, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.L_Knee, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.L_Ancle, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.L_Toe, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_Thigh, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_Knee, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_Ancle, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_Toe, damper, spring_ratio);
            }
            {   /* tales */
                float damper = 10f;
                float spring_ratio = 200f;
                muscle_motion_.setParams(MuscleMotion.Parts.L_Tale1, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.L_Tale2, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.L_Tale3, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.L_Tale4, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_Tale1, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_Tale2, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_Tale3, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_Tale4, damper, spring_ratio);
            }
            {   /* ribbons */
                float damper = 2f;
                float spring_ratio = 100f;
                muscle_motion_.setParams(MuscleMotion.Parts.L_Ribbon1, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.L_Ribbon2, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_Ribbon1, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_Ribbon2, damper, spring_ratio);
            }
            {   /* suso */
                float damper = 10f;
                float spring_ratio = 200f;
                muscle_motion_.setParams(MuscleMotion.Parts.L_SusoBack, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.L_SusoFront, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_SusoBack, damper, spring_ratio);
                muscle_motion_.setParams(MuscleMotion.Parts.R_SusoFront, damper, spring_ratio);
            }
        }

        private void fire(Bullet bullet, double update_time)
        {
            const float BULLET_SPEED = 40f;
            var velocity = bullet.rigidbody_.transform_.rotation_ * CV.Vector3Forward * BULLET_SPEED;
            bullet.release(ref velocity, update_time);
            bullet.setPower(.25f);
            SystemManager.Instance.registSound(DrawBuffer.SE.Bullet);
        }
        private void fire_left(double update_time)
        {
            if (left_held_bullet_ != null)
            {
                fire(left_held_bullet_, update_time);
                left_held_bullet_ = null;
            }
            bullet_tame_left_ = 0f;
        }
        private void fire_right(double update_time)
        {
            if (right_held_bullet_ != null)
            {
                fire(right_held_bullet_, update_time);
                right_held_bullet_ = null;
            }
            bullet_tame_right_ = 0f;
        }

        public void Fire_left(ref Vector3 pos, ref Quaternion rotation)
        {
            fire_left(1 / 60f);
            left_held_bullet_ = Bullet.create(ref pos,
                                              ref rotation);
        }
        public void Fire_right(ref Vector3 pos, ref Quaternion rotation)
        {
            fire_right(1 / 60f);
            right_held_bullet_ = Bullet.create(ref pos,
                                            ref rotation);
        }

        public override void update(float dt, double update_time)
        {
            Vector3 pos = robotT.position;
            rigidbody_.setPosition(ref pos);
            rigidbody_.update(dt);
            var intersect_point = CV.Vector3Zero;
            if (MyCollider.takeEnemyDamageForPlayer(collider_, ref intersect_point) == MyCollider.Type.EnemyBullet)
            {
                Shield.Instance.spawn(ref intersect_point,
                                      ref rigidbody_.transform_.position_,
                                      update_time,
                                      Shield.Type.Green);
                SystemManager.Instance.registSound(DrawBuffer.SE.Shield);
                // hit_time_ = (float)update_time;
                // hit_position_ = intersect_point;
            }
            MyCollider.updatePlayer(collider_, ref pos);
            fire_right(update_time);
            fire_left(update_time);

            if (rigidbody_.transform_.position_.y < 5f)
            {
                WaterSurface.Instance.makeBump(ref rigidbody_.transform_.position_, -0.05f /* value */, 0.6f /* size */);
                pos = rigidbody_.transform_.position_;
                pos.y = -2f;
                float vel_y;
                vel_y = MyRandom.Range(5f, 7f);
                var vel = new Vector3(0f,
                                      vel_y,
                                      0f);
                if (MyRandom.Probability(0.2f))
                {
                    WaterSplash.Instance.spawn(ref pos, ref vel, update_time);
                }
            }
            on_ground_time_ += dt;
        }

        public override void renderUpdate(int front, CameraBase camra, ref DrawBuffer draw_buffer)
        {
            muscle_motion_.renderUpdate(ref draw_buffer, DrawBuffer.Type.MuscleMotionPlayer);
        }
    }

} // namespace UTJ {

/*
 * End of Player.cs
 */
