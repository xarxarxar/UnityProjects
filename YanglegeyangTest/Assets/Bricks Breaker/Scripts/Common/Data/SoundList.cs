using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SoundList {

    //Company logo appeared
    public static string ball_bounce = "ball_bounce";

    //Common: Basic Entry, Basic Out, Sub Button Sound
    public static string sound_common_btn_in = "sound_common_btn_in";
    public static string sound_common_btn_close = "sound_common_btn_close";
    public static string sound_common_btn_sub = "sound_common_btn_sub";

    //Background Music: Title
    public static string sound_home_bgm = "sound_home_bgm";

    //Background Music: Play
    public static string sound_play_bgm {
        get {
            string name = "sound_play_bgm" + GameData.BgmCount;

            return name;
        }
    }


    public static string sound_play_sfx_in = "sound_play_sfx_in";

    //Play: When the ball hits the brick
    public static string sound_play_sfx_ball = "sound_play_sfx_ball";
    //Play: When the ball fires
    public static string sound_play_sfx_ball_launch = "sound_play_sfx_ball_launch";
    //Play: When you get the ball
    public static string sound_play_sfx_addball = "sound_play_sfx_addball";
    //Play: When the block comes down
    public static string sound_play_sfx_block_down = "sound_play_sfx_block_down";
    //Play: When the ball comes down to the bottom line
    public static string sound_play_sfx_ball_comback = "sound_play_sfx_ball_comback";
    //Play: When the last ball goes down the bottom line
    public static string sound_play_sfx_ball_comback_last = "sound_play_sfx_ball_comback_last";



    //Left and right swipe
    public static string sound_common_sfx_swipe = "sound_common_sfx_swipe";
    //Equip
    public static string sound_skin_btn_equip = "sound_skin_btn_equip";
    //Level up
    public static string sound_result_sfx_levelup = "sound_result_sfx_levelup";
    //Coin Acquisition
    public static string sound_common_sfx_coin = "sound_common_sfx_coin";
    //Jewelry acquisition
    public static string sound_common_sfx_gem = "sound_common_sfx_gem";
    //When there are not enough gems and coins
    public static string sound_common_sfx_error = "sound_common_sfx_error";
    //Acquire skin
    public static string sound_common_sfx_get_skinmp3 = "sound_common_sfx_get_skinmp3";


    //Play: When you get Lucky Bonus
    public static string sound_play_sfx_lucky = "sound_play_sfx_lucky";
    
    //Play: All-Clear Bonus
    public static string sound_play_sfx_allclearbonus0 = "sound_play_sfx_allclearbonus0";
    public static string sound_play_sfx_allclearbonus1 = "sound_play_sfx_allclearbonus1";
    public static string sound_play_sfx_allclearbonus2 = "sound_play_sfx_allclearbonus2";



    //Play: When collecting the ball
    public static string sound_play_common_sfx_ballcollect = "sound_play_common_sfx_ballcollect";
    
    //Play: Destruction sound when over 5 combos
    public static string sound_play_sfx_block_destroy_big = "sound_play_sfx_block_destroy_big";




    //Play: When the block is destroyed 1
    public static string sound_play_sfx_block1_destory {
        get {
            string name = "sound_play_sfx_block1_destory" + Random.Range(0, 3);
            return name;
        }
    }

    //Play: when the block is destroyed 2
    public static string sound_play_sfx_block2_destory {
        get {
            string name = "sound_play_sfx_block2_destory" + Random.Range(0, 3);
            return name;
        }
    }

    //Play: when the block is destroyed 3
    public static string sound_play_sfx_block3_destory {
        get {
            string name = "sound_play_sfx_block3_destory" + Random.Range(0, 3);
            return name;
        }
    }

    //Play: when the block is destroyed 4
    public static string sound_play_sfx_block4_destory {
        get {
            string name = "sound_play_sfx_block4_destory" + Random.Range(0, 3);
            return name;
        }
    }

    //Play: when the block is destroyed 5
    public static string sound_play_sfx_block5_destory {
        get {
            string name = "sound_play_sfx_block5_destory" + Random.Range(0, 3);
            return name;
        }
    }

    //Play: when the block is destroyed 6
    public static string sound_play_sfx_block6_dsstory {
        get {
            string name = "sound_play_sfx_block5_destory" + Random.Range(0, 3);
            return name;
        }
    }

    //Home: Start button
    public static string sound_home_btn_start = "sound_home_btn_start";

    //Stop Play: When touch the stop button
    public static string sound_play_sfx_pause = "sound_play_sfx_pause";

    // Stop playing: When you play again
    public static string sound_play_sfx_resume = "sound_play_sfx_resume";

    //Exit: Following pop-up exposure
    public static string sound_continue_sfx_default = "sound_continue_sfx_default";

    //End: When it is a new record
    public static string sound_gameover_sfx_newhighscore = "sound_gameover_sfx_newhighscore";

    //End: When the game is completely over
    public static string sound_gameover_sfx_default = "sound_gameover_sfx_default";

    //End: When the advertisement report is revived
    public static string sound_play_sfx_revival = "sound_play_sfx_revival";

    //Acquire points
    public static string sound_result_sfx_score = "sound_result_sfx_score";
    
    // Camera shutter
    public static string sound_gameover_sfx_cameraflash {
        get {
            string name = "sound_gameover_sfx_cameraflash" + Random.Range(0, 5);
            return name;
        }
    }

    //Play: Upon completion of the rocket skill cooldown
    public static string sound_rocket_sfx_cool = "sound_rocket_sfx_cool";

    //Play: Ready to fire rocket skills
    public static string sound_rocket_sfx_ready = "sound_rocket_sfx_ready";

    //Play: When launching rocket skill
    public static string sound_rocket_sfx_launch = "sound_rocket_sfx_launch";
}
