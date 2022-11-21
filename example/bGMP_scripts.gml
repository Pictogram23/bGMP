#define bGMP_startup
/***************************************************
  return
    true: Ok
    false: error
 ***************************************************/

global.bGMP_exe_filepath = "bGMP.exe";
global.bGMP_exe_password = "";
global.bGMP_ini_filepath = "bGMP.ini";
global.bGMP_wait_time = 10;
global.bGMP_track_count = 10;
global.bGMP_volume_bottom_frac = 100;

if !file_exists(global.bGMP_exe_filepath) {
    return false;
}

if variable_global_exists("bGMP_startup_has_called") {
    return true;
}

if file_exists(global.bGMP_ini_filepath) {
    file_delete(global.bGMP_ini_filepath);
}

execute_program(global.bGMP_exe_filepath,global.bGMP_exe_password,false);

while !file_exists(global.bGMP_ini_filepath) {
    sleep(global.bGMP_wait_time);
};

global.bGMP_startup_has_called = true;

bGMP_read();
bGMP_init_params();

return true;

#define bGMP_end
if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if global.bGMP_startup_has_called == false {
    exit;
}

if file_exists(global.bGMP_ini_filepath) {
    file_delete(global.bGMP_ini_filepath);
    global.bGMP_startup_has_called = false;
}

#define bGMP_read
if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}

var section,tc;

section = "";
tc = global.bGMP_track_count;

ini_open("bGMP.ini");

for (i = 0; i < tc; i += 1)
{
    if (0 <= i && i < 10)
    {
        section = "Track0" + string(i);
    }
    else if (10 <= i < 100)
    {
        section = "Track" + string(i);
    }
    global.bGMP_audio_id[i] = ini_read_string(section,"id","");
    global.bGMP_audio_isend[i] = ini_read_real(section,"isEnd",false);
    global.bGMP_audio_isplaying[i] = ini_read_real(section,"isPlay",false);
    global.bGMP_audio_isloop[i] = ini_read_real(section,"isLoop",false);
    global.bGMP_audio_volume[i] = ini_read_real(section,"volume",1);
    global.bGMP_audio_pitch[i] = ini_read_real(section,"pitch",0);
    global.bGMP_audio_tempo[i] = ini_read_real(section,"tempo",1);
    global.bGMP_audio_rate[i] = ini_read_real(section,"rate",1);
    global.bGMP_audio_pan[i] = ini_read_real(section,"pan",0);
    global.bGMP_audio_position[i] = ini_read_real(section,"position",-1)
    global.bGMP_audio_length[i] = ini_read_real(section,"length",0);
}

ini_close();

#define bGMP_init_params
if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}

var section,tc;

section = "";
tc = global.bGMP_track_count;

ini_open("bGMP.ini");

for (i = 0; i < tc; i += 1)
{
    if (0 <= i && i < 10)
    {
        section = "Track0" + string(i);
    }
    else if (10 <= i < 100)
    {
        section = "Track" + string(i);
    }
    ini_write_string(section, "id", global.bGMP_audio_id[i]);
    ini_write_real(section, "isEnd", global.bGMP_audio_isend[i]);
    ini_write_real(section, "isPlay", global.bGMP_audio_isplaying[i]);
    ini_write_real(section, "isLoop", global.bGMP_audio_isloop[i]);
    ini_write_real(section, "volume", global.bGMP_audio_volume[i]);
    ini_write_real(section, "pitch", global.bGMP_audio_pitch[i]);
    ini_write_real(section, "tempo", global.bGMP_audio_tempo[i]);
    ini_write_real(section, "rate", global.bGMP_audio_rate[i]);
    ini_write_real(section, "pan", global.bGMP_audio_pan[i]);
    ini_write_real(section, "position", global.bGMP_audio_position[i]);
    ini_write_real(section, "length", global.bGMP_audio_length[i]);
}

section = "Global";
ini_write_real(section, "TrackCount", global.bGMP_track_count);

ini_close();

#define bGMP_load
// bGMP_load(trackNum, audioId);

if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}

var trackNum,audioId,section;

trackNum = argument0;
audioId = argument1;
section = "";

if (global.bGMP_audio_id[trackNum] == "") {
    if 0 <= trackNum && trackNum < 10 {
        section = "Track0" + string(floor(trackNum));
    } else if 10 <= trackNum {
        section = "Track" + string(floor(trackNum));
    }
    
    global.bGMP_audio_id[trackNum] = audioId;
    
    ini_open(global.bGMP_ini_filepath);
    ini_write_string(section,"id",global.bGMP_audio_id[trackNum]);
    ini_write_real(section,"isEnd",false);
    while true {
        if global.bGMP_audio_length[trackNum] != ini_read_real(section,"length",0) {
            global.bGMP_audio_length[trackNum] = ini_read_real(section,"length",0);
            break;
        }
        sleep(global.bGMP_wait_time);
    }
    ini_close();
}

#define bGMP_play
// bGMP_play(trackNum)

if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}

var trackNum,section;
trackNum = argument0;
section = "";

if global.bGMP_audio_id[trackNum] != "" && !global.bGMP_audio_isplaying[trackNum] {
    if 0 <= trackNum && trackNum < 10 {
        section = "Track0" + string(floor(trackNum));
    } else if 10 <= trackNum {
        section = "Track" + string(floor(trackNum));
    }
    
    global.bGMP_audio_isplaying[trackNum] = true;
    
    ini_open(global.bGMP_ini_filepath);
    ini_write_real(section,"isPlay",global.bGMP_audio_isplaying[trackNum]);
    ini_close();
}

#define bGMP_stop
// bGMP_stop(trackNum);

if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}

var trackNum,section;
trackNum = argument0;
section = "";
    
if global.bGMP_audio_id[trackNum] != "" && global.bGMP_audio_isplaying[trackNum] {
    if 0 <= trackNum && trackNum < 10 {
        section = "Track0" + string(floor(trackNum));
    } else if 10 <= trackNum {
        section = "Track" + string(floor(trackNum));
    }
    
    global.bGMP_audio_isplaying[trackNum] = false;
        
    ini_open(global.bGMP_ini_filepath);
    ini_write_real(section,"isPlay",global.bGMP_audio_isplaying[trackNum]);
    ini_close();
}

#define bGMP_close
// bGMP_close(trackNum);

if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}
    
var trackNum,section;
trackNum = argument0;
section = "";

if global.bGMP_audio_id[trackNum] == "" {
    exit;
}

if 0 <= trackNm && trackNum < 10 {
    section = "Track0" + string(floor(trackNum));
} else if 10 <= trackNum {
    section = "Track" + string(floor(trackNum));
}

ini_open(global.bGMP_ini_filepath);

global.bGMP_audio_isplaying[trackNum] = false;
ini_write_real(section,"isPlay",global.bGMP_audio_isplaying[trackNum]);
global.bGMP_audio_isloop[trackNum] = false;
ini_write_real(section,"isLoop",global.bGMP_audio_isloop[trackNum]);
global.bGMP_audio_length[trackNum] = 0;
ini_write_real(section,"length",global.bGMP_audio_length[trackNum]);
global.bGMP_audio_position[trackNum] = -1;
ini_write_real(section,"position",global.bGMP_audio_position[trackNum]);

global.bGMP_audio_id[trackNum] = "";
ini_write_string(section,"id",global.bGMP_audio_id[trackNum]);

ini_close();

while true {
    ini_open(global.bGMP_ini_filepath);
    if global.bGMP_audio_isend[trackNum] != ini_read_real(section,"isEnd",false) {
        global.bGMP_audio_isend[trackNum] = ini_read_real(section,"isEnd",false);
        break;
    }
    ini_close();
    sleep(global.bGMP_wait_time);    
}

#define bGMP_set_loop
// bGMP_set_loop(trackNum,isLoop);

if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}

var trackNum,isLoop,section;
trackNum = argument0;
isLoop = argument1;
section = "";

if global.bGMP_audio_id[trackNum] == "" {
    exit;
}

if global.bGMP_audio_isloop[trackNum] == isLoop {
    exit;
}

if 0 <= trackNum && trackNum < 10 {
    section = "Track0" + string(floor(trackNum));
} else if 10 <= trackNum {
    section = "Track" + string(floor(trackNum));
}

global.bGMP_audio_isloop[trackNum] = isLoop;
ini_open(global.bGMP_ini_filepath);
ini_write_real(section,"isLoop",global.bGMP_audio_isloop[trackNum]);
ini_close();

#define bGMP_set_volume
// bGMP_set_volume(trackNum,val);

if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}

var trackNum,val,section;
trackNum = argument0;
val = argument1 / global.bGMP_volume_bottom_frac;
section = "";

if global.bGMP_audio_id[trackNum] == "" {
    exit;
}

if val < 0 || 1 < val {
    exit;
}

if global.bGMP_audio_volume[trackNum] == val {
    exit;
}

if 0 <= trackNum && trackNum < 10 {
    section = "Track0" + string(floor(trackNum));
} else if 10 <= trackNum {
    section = "Track" + string(floor(trackNum));
}

global.bGMP_audio_volume[trackNum] = val;
ini_open(global.bGMP_ini_filepath);
ini_write_real(section,"volume",global.bGMP_audio_volume[trackNum]);
ini_close();


#define bGMP_set_pitch
// bGMP_set_pitch(trackNum,val);

if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}

var trackNum,val,section;
trackNum = argument0;
val = argument1;
section = "";

if global.bGMP_audio_id[trackNum] == "" {
    exit;
}

if global.bGMP_audio_pitch[trackNum] == val {
    exit;
}

if 0 <= trackNum && trackNum < 10 {
    section = "Track0" + string(floor(trackNum));
} else if 10 <= trackNum {
    section = "Track" + string(floor(trackNum));
}

global.bGMP_audio_pitch[trackNum] = val;
ini_open(global.bGMP_ini_filepath);
ini_write_real(section,"pitch",global.bGMP_audio_pitch[trackNum]);
ini_close();


#define bGMP_set_tempo
// bGMP_set_pitch(trackNum,val);

if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}

var trackNum,val,section;
trackNum = argument0;
val = argument1;
section = "";

if global.bGMP_audio_id[trackNum] == "" {
    exit;
}

if global.bGMP_audio_tempo[trackNum] == val {
    exit;
}

if 0 <= trackNum && trackNum < 10 {
    section = "Track0" + string(floor(trackNum));
} else if 10 <= trackNum {
    section = "Track" + string(floor(trackNum));
}

global.bGMP_audio_tempo[trackNum] = val;
ini_open(global.bGMP_ini_filepath);
ini_write_real(section,"tempo",global.bGMP_audio_tempo[trackNum]);
ini_close();


#define bGMP_set_rate
// bGMP_set_pitch(trackNum,val);

if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}

var trackNum,val,section;
trackNum = argument0;
val = argument1;
section = "";

if global.bGMP_audio_id[trackNum] == "" {
    exit;
}

if global.bGMP_audio_rate[trackNum] == val {
    exit;
}

if 0 <= trackNum && trackNum < 10 {
    section = "Track0" + string(floor(trackNum));
} else if 10 <= trackNum {
    section = "Track" + string(floor(trackNum));
}

global.bGMP_audio_rate[trackNum] = val;
ini_open(global.bGMP_ini_filepath);
ini_write_real(section,"rate",global.bGMP_audio_rate[trackNum]);
ini_close();


#define bGMP_set_pan
// bGMP_set_pitch(trackNum,val);

if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}

var trackNum,val,section;
trackNum = argument0;
val = argument1;
section = "";

if global.bGMP_audio_id[trackNum] == "" {
    exit;
}

if val < -1 or 1 < val {
    exit;
}

if global.bGMP_audio_pan[trackNum] == val {
    exit;
}

if 0 <= trackNum && trackNum < 10 {
    section = "Track0" + string(floor(trackNum));
} else if 10 <= trackNum {
    section = "Track" + string(floor(trackNum));
}

global.bGMP_audio_pan[trackNum] = val;
ini_open(global.bGMP_ini_filepath);
ini_write_real(section,"pan",global.bGMP_audio_pan[trackNum]);
ini_close();


#define bGMP_set_position
// bGMP_set_pitch(trackNum,val);

if !variable_global_exists("bGMP_startup_has_called") {
    exit;
}

if !global.bGMP_startup_has_called {
    exit;
}

var trackNum,val,section;
trackNum = argument0;
val = argument1;
section = "";

if global.bGMP_audio_id[trackNum] == "" {
    exit;
}

if global.bGMP_audio_position[trackNum] == val {
    exit;
}

if 0 <= trackNum && trackNum < 10 {
    section = "Track0" + string(floor(trackNum));
} else if 10 <= trackNum {
    section = "Track" + string(floor(trackNum));
}

global.bGMP_audio_position[trackNum] = val;
ini_open(global.bGMP_ini_filepath);
ini_write_real(section,"position",global.bGMP_audio_position[trackNum]);
ini_close();


