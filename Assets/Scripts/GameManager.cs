/* -*- mode:CSharp; coding:utf-8-with-signature -*-
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
            yield return null;
        }
    }

} // namespace UTJ {

/*
 * End of GameManager.cs
 */
