﻿/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UTJ
{

    public class GameManager
    {
        // singleton
        static GameManager instance_;
        public static GameManager Instance { get { return instance_ ?? (instance_ = new GameManager()); } }

        private IEnumerator enumerator_;
        private double update_time_;

        public void init()
        {
            enumerator_ = act();    // この瞬間は実行されない
        }

        public void update(float dt, double update_time)
        {
            update_time_ = update_time;
            enumerator_.MoveNext();
        }

        public void restart()
        {
            enumerator_ = null;
            enumerator_ = act();
        }

        private IEnumerator act()
        {
            // SystemManager.Instance.registBgm(DrawBuffer.BGM.Battle);
            {
                float x = MyRandom.Range(5, 10) * (Random.Range(0, 2) == 0 ? 1 : -1);
                var position = new Vector3(x, -10f, 20f) + Player.Instance.rigidbody_.transform_.position_;
                var rotation = Quaternion.Euler(-90f, 0f, 0f);
                Enemy.create(Enemy.Type.Dragon, ref position, ref rotation);
            }
            for (;;)
            {
                {
                    float x = MyRandom.Range(5, 10) * (Random.Range(0, 2) == 0 ? 1 : -1);
                    var position = new Vector3(x, -10f, 20f) + Player.Instance.rigidbody_.transform_.position_;
                    var rotation = Quaternion.Euler(-90f, 0f, 0f);
                    Enemy.create(Enemy.Type.Zako, ref position, ref rotation);
                }
                for (var i = new Utility.WaitForSeconds(MyRandom.Range(1f, 3f), update_time_); !i.end(update_time_);)
                {
                    yield return null;
                }
                yield return null;
            }
        }
    }

} // namespace UTJ {

/*
 * End of GameManager.cs
 */
