/* -*- mode:CSharp:utf-8-with-signature -*-
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

public class Player : Task {
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

	public MuscleMotion getMuscleMotion() { return muscle_motion_; }
	public float TameGaugeValue() { return Mathf.Clamp(jump_tame_duration_*2f, 0f, 1f); }
	public float getAuraValue() { return jump_tame_duration_ > 0f || !on_ground_ ? 1f : 0f; }

	public IEnumerator initialize()
	{
		base.init();

            yield return null;

		collider_ = MyCollider.createPlayer();
		MyCollider.initSpherePlayer(collider_, ref rigidbody_.transform_.position_, 1f /* radius */);
	} 

	public override void update(float dt, double update_time)
	{ 
	}

	public override void renderUpdate(int front, CameraBase camra, ref DrawBuffer draw_buffer)
	{

	}
}

} // namespace UTJ {

/*
 * End of Player.cs
 */
