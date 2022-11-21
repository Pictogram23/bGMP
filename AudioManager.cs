using System.Text;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Vorbis;
using SoundTouch.Net.NAudioSupport;
using Microsoft.Win32.SafeHandles;
using IniParser;
using static bGMP.GlobalParams;
using System.Reflection;
using System;
using System.IO;

namespace bGMP
{
    class AudioManager : IDisposable
    {
        SoundTouchWaveStream _sound;
        SampleChannel _sample;
        WaveOutEvent _waveOut;
        public int _myIndex;
        bool _isLoop = false;
        bool _isPause = false;
        /*public*/
        int _isEnd = 0;
        WaveChannel32 waveChannel;

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            SafeFileHandle handle = new SafeFileHandle(IntPtr.Zero, true);

            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                handle.Dispose();
            }

            _disposed = true;
        }

        public void audioLoad(string name, string format)
        {
            if (_waveOut != null) audioClose();
            waveChannel = null;
            getAudioResource(name, format);
            if (waveChannel != null)
            {
                _sound = new SoundTouchWaveStream(waveChannel);
                _sample = new SampleChannel(_sound);
                _waveOut = new WaveOutEvent() { DesiredLatency = 100 };
                _waveOut.PlaybackStopped += endHandler;
                _waveOut.Init(_sample);
            }
            EditIniData(KEY_LENGTH, _sound.Length.ToString());
        }

        void endHandler(object source, EventArgs e)
        {
            if (_sound == null)
                return;
            if (_isPause == false)
            {
                if (_isLoop == true)
                {
                    _waveOut.PlaybackStopped -= endHandler;
                    waveChannel.Position = 0;
                    _waveOut.Play();
                    _waveOut.PlaybackStopped += endHandler;
                }
                else
                {
                    _isEnd = 1;
                    EditIniData(KEY_ISEND, _isEnd.ToString());
                }
            }
        }

        public void audioPlay()
        {
            if (_waveOut != null && _waveOut.PlaybackState != PlaybackState.Playing)
            {
                _waveOut.Play();
                _isPause = false;
            }
        }

        public void audioStop()
        {
            if (_waveOut != null && _waveOut.PlaybackState == PlaybackState.Playing)
            {
                _isPause = true;
                _waveOut.Stop();
            }
        }

        public void audioClose()
        {
            _sound?.Dispose();
            _sound = null;

            _sample = null;

            _waveOut?.Dispose();
            _waveOut = null;

            _isPause = false;

            _isEnd = 1;
            EditIniData(KEY_ISEND, _isEnd.ToString());
        }

        public void audioSetVolume(float volume)
        {
            if (_sample != null && volume <= 1.0f && volume >= 0.0f) _sample.Volume = volume;
        }

        public void audioSetPitch(int pitch)
        {
            if (_sound != null && pitch >= -12 && pitch <= 12)
                _sound.PitchSemiTones = pitch;
        }

        public void audioSetTempo(int tempo)
        {
            if (_sound != null && tempo >= -50 && tempo <= 100)
                _sound.TempoChange = tempo;
        }

        public void audioSetRate(int rate)
        {
            if (_sound != null && rate >= -50 && rate <= 100)
                _sound.RateChange = rate;
        }

        public void audioSetLoop(int isLoop)
        {
            if (isLoop == 0)
                _isLoop = false;
            else
                _isLoop = true;
        }

        public void audioSetPan(float pan)
        {
            if (waveChannel != null)
                waveChannel.Pan = pan;
        }

        public void audioSetPotision(long position)
        {
            if (_sound != null)
                _sound.Position = position;
            EditIniData(KEY_POSITION, "-1");
        }

        private void EditIniData(string key, string data)
        {
            try
            {
                string section;
                var parser = new FileIniDataParser();

                parser.Parser.Configuration.AssigmentSpacer = ""; // NEW
                var iniData = parser.ReadFile(BGMP_INI);

                if (_myIndex < 9)
                    section = SECTION_TRACK + "0" + _myIndex.ToString();
                else
                    section = SECTION_TRACK + _myIndex.ToString();

                iniData[section][key] = data;
                parser.WriteFile(BGMP_INI, iniData, Encoding.Unicode);
            }
            catch
            {
                EditIniData(key, data);
            }
        }

        private void getAudioResource(string name, string format)
        {
            var assembly = Assembly.GetExecutingAssembly();

            //var Resource = new ResXResourceSet(RESX_FILE_PATH);
            //System.IO.Stream resource = null;
            string resource = "bGMP.resources." + name;
            Stream stream = assembly.GetManifestResourceStream(resource);
            if (stream == null)
            {
                Environment.Exit(0);
            }
            switch (format)
            {
                case "wav":
                    var wavStream = assembly.GetManifestResourceStream(resource);
                    var wave = new WaveFileReader(stream);
                    waveChannel = new WaveChannel32(wave) { PadWithZeroes = false };
                    break;
                case "mp3":
                    var mp3Stream = assembly.GetManifestResourceStream(resource);
                    var mp3 = new Mp3FileReader(stream);
                    waveChannel = new WaveChannel32(mp3) { PadWithZeroes = false };
                    break;
                case "ogg":
                    var vorbisStream = assembly.GetManifestResourceStream(resource);
                    var vorbis = new VorbisWaveReader(stream);
                    waveChannel = new WaveChannel32(vorbis) { PadWithZeroes = false };
                    break;
                case "aiff":
                    var aiffStream = assembly.GetManifestResourceStream(resource);
                    var aiff = new AiffFileReader(stream);
                    waveChannel = new WaveChannel32(aiff) { PadWithZeroes = false };
                    break;
                default:
                    waveChannel = null;
                    _sound = null;
                    _sample = null;
                    _waveOut = null;
                    break;
            }
        }
    }
}
