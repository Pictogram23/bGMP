using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using IniParser;
using IniParser.Model;
using static bGMP.GlobalParams;

namespace bGMP
{
    class BgmpFileManager
    {

        string[] id = new string[TRACK_COUNT];
        int[] isPlay = new int[TRACK_COUNT];
        int[] isLoop = new int[TRACK_COUNT];
        float[] volume = new float[TRACK_COUNT];
        int[] pitch = new int[TRACK_COUNT];
        int[] tempo = new int[TRACK_COUNT];
        int[] rate = new int[TRACK_COUNT];
        float[] pan = new float[TRACK_COUNT];
        long[] position = new long[TRACK_COUNT];
        long[] length = new long[TRACK_COUNT];

        AudioManager[] _audioManager = new AudioManager[TRACK_COUNT];

        /// <summary>
        /// 音声ファイルのパスと音声ファイルの形式を受け取り、bgmpファイルから読み取った情報をもとに
        ///     音声ファイル操作の命令を下したり、bgmpファイルを管理する
        /// 
        /// Get The Audio File's Path and Format. Also, Command The Operation for it and,
        ///     Manage bgmp File, from bgmp File's infomation
        /// </summary>

        FileSystemWatcher watcher1/*, watcher2*/;

        public BgmpFileManager()
        {
            /*string section = SECTION_GLOBAL;
            var parser = new FileIniDataParser();*/
            while (true)
            {
                try
                {
                    if (isFileLocked(BGMP_INI) == false)
                    {
                        var parser = new FileIniDataParser();
                        IniData resData = new IniData();
                        resData.Sections.AddSection(SECTION_GLOBAL);
                        resData[SECTION_GLOBAL].AddKey(HAS_CALLED, "1");
                        parser.WriteFile(BGMP_TMP_INI, resData);
                        break;
                    }
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }

            watcher1 = new FileSystemWatcher();
            watcher1.Path = @"./";
            watcher1.Filter = BGMP_INI;
            watcher1.Created += on_created_bgmpini;
            watcher1.Changed += on_changed_bgmpini;
            watcher1.Deleted += on_deleted_bgmpini;
            watcher1.EnableRaisingEvents = true;

            /*
            watcher2 = new FileSystemWatcher();
            watcher2.Path = @".\";
            watcher2.Filter = BGMP_TMP_INI;
            watcher2.Created += on_created_bgmpini;
            watcher2.EnableRaisingEvents = true;*/
        }

        public void bgmp_delete()
        {
            File.Delete(BGMP_INI);
            //File.Delete(BGMP_TMP_INI);
        }

        public void on_created_bgmpini(object source, FileSystemEventArgs e)
        {
            execute_bgmpfile_operation();
            //watcher1.EnableRaisingEvents = true;
        }

        public void on_changed_bgmpini(object source, FileSystemEventArgs e)
        {
            execute_bgmpfile_operation();
            //watcher1.EnableRaisingEvents = true;
        }

        public void on_deleted_bgmpini(object source, FileSystemEventArgs e)
        {
            Environment.Exit(0);
        }

        public void execute_bgmpfile_operation()
        {
            var parser = new FileIniDataParser();
            try
            {
                var data = parser.ReadFile(BGMP_INI);

                string section;

                for (int i = 0; i < TRACK_COUNT; i++)
                {
                    if (i < 9)
                        section = SECTION_TRACK + "0" + i.ToString();
                    else
                        section = SECTION_TRACK + i.ToString();

                    string tmpId = data[section][KEY_MUSICID];
                    if (id[i] != tmpId)
                    {
                        id[i] = tmpId;

                        string title;
                        string format; // 再生するファイルの形式 Format of The Audio File of Want to Play

                        string musicId = id[i];

                        if (id[i] != "")
                        {
                            string[] splited = musicId.Split('_');
                            if (splited.Length >= 2)
                            {
                                title = splited[0] + "_" + splited[1];
                                format = splited[1];

                                if (format != "")
                                {
                                    _audioManager[i] = new AudioManager();
                                    _audioManager[i]._myIndex = i;
                                    _audioManager[i].audioLoad(title, format);
                                    _audioManager[i].audioSetLoop(int.Parse(data[section][KEY_ISLOOP]));
                                    _audioManager[i].audioSetVolume(float.Parse(data[section][KEY_VOLUME]));
                                    _audioManager[i].audioSetPitch(int.Parse(data[section][KEY_PITCH]));
                                    _audioManager[i].audioSetTempo(int.Parse(data[section][KEY_TEMPO]));
                                    _audioManager[i].audioSetRate(int.Parse(data[section][KEY_RATE]));
                                    _audioManager[i].audioSetPan(int.Parse(data[section][KEY_PAN]));
                                    //_audioManager[i].audioSetPotision(long.Parse(data[section][KEY_POSITION]));
                                    _audioManager[i].audioSetPotision(DEF_POSITION);
                                }
                            }
                        }
                        else
                        {
                            _audioManager[i].audioClose();
                        }

                    }

                    int tmpIsPlay;
                    int.TryParse(data[section][KEY_ISPLAY], out tmpIsPlay);
                    if (isPlay[i] != tmpIsPlay && _audioManager[i] != null)
                    {
                        isPlay[i] = tmpIsPlay;
                        if (isPlay[i] == 0)
                        {
                            _audioManager[i].audioStop();
                        }
                        else if (isPlay[i] == 1)
                        {
                            _audioManager[i].audioPlay();
                        }
                    }

                    int tmpIsLoop;
                    int.TryParse(data[section][KEY_ISLOOP], out tmpIsLoop);
                    if (isLoop[i] != tmpIsLoop && _audioManager[i] != null)
                    {
                        isLoop[i] = tmpIsLoop;
                        _audioManager[i].audioSetLoop(isLoop[i]);
                    }

                    float tmpVolume;
                    float.TryParse(data[section][KEY_VOLUME], out tmpVolume);
                    if (volume[i] != tmpVolume && _audioManager[i] != null)
                    {
                        volume[i] = tmpVolume;
                        _audioManager[i].audioSetVolume(volume[i]);
                    }

                    int tmpPitch;
                    int.TryParse(data[section][KEY_PITCH], out tmpPitch);
                    if (pitch[i] != tmpPitch && _audioManager[i] != null)
                    {
                        pitch[i] = tmpPitch;
                        _audioManager[i].audioSetPitch(pitch[i]);
                    }

                    int tmpTempo;
                    int.TryParse(data[section][KEY_TEMPO], out tmpTempo);
                    if (tempo[i] != tmpTempo && _audioManager[i] != null)
                    {
                        tempo[i] = tmpTempo;
                        _audioManager[i].audioSetTempo(tempo[i]);
                    }

                    int tmpRate;
                    int.TryParse(data[section][KEY_RATE], out tmpRate);
                    if (rate[i] != tmpRate && _audioManager[i] != null)
                    {
                        rate[i] = tmpRate;
                        _audioManager[i].audioSetRate(rate[i]);
                    }

                    float tmpPan;
                    float.TryParse(data[section][KEY_PAN], out tmpPan);
                    if (pan[i] != tmpPan && _audioManager[i] != null)
                    {
                        pan[i] = tmpPan;
                        _audioManager[i].audioSetPan(pan[i]);
                    }

                    long tmpPosition;
                    long.TryParse(data[section][KEY_POSITION], out tmpPosition);
                    if (position[i] != tmpPosition && _audioManager[i] != null)
                    {
                        position[i] = tmpPosition;
                        _audioManager[i].audioSetPotision(position[i]);
                    }
                }
                //File.Delete(BGMP_TMP_INI);
            }
            catch
            {
                return;
            }
        }

        private List<string> readFileText(string filepath)
        {
            List<string> text = new List<string>();
            try
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    while (reader.Peek() >= 0)
                    {
                        text.Add(reader.ReadLine());
                    }
                    reader.Close();
                }
            }
            catch //(IOException ex)
            {
                //Console.WriteLine(ex.Message);
                System.Threading.Thread.Sleep(10);
                return readFileText(filepath);
            }
            return text;
        }

        bool isFileLocked(string filename)
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return false;
        }
    }
}
