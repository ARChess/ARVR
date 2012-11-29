using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Speech.Synthesis;
using System.IO;
using System.Speech.Recognition;
using System.Speech.AudioFormat;
using System.Threading;

namespace ARVR
{
    public class ARVR : IARVR
    {
        private Grammar _grammar = null;

        public string RecognizeSpeech(byte[] speechToParse, int sampleRate)
        {
            SpeechRecognitionEngine sre = new SpeechRecognitionEngine();

            if (_grammar == null)
                InitializeGrammar();
            sre.LoadGrammar(_grammar);

            MemoryStream ms = new MemoryStream(speechToParse);
            var formatInfo = new SpeechAudioFormatInfo(sampleRate, AudioBitsPerSample.Sixteen, AudioChannel.Mono);
            sre.SetInputToAudioStream(ms, formatInfo);
            var result = sre.Recognize();
            ms = null;

            if (result == null)
                return "Unable to recognize speech";
            else
                return result.Text;
        }

        void sre_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {

        }

        private void InitializeGrammar()
        {
            //[select <piece> at <x><y> | move <piece> to <x><y>]

            //build the core set of choices
            Choices action = new Choices("select", "move", "select the", "move the");
            Choices pieces = new Choices("pawn", "rook", "bishop", "knight", "king", "queen");
            Choices x_positions = new Choices("A", "B", "C", "D", "E", "F", "G", "H");
            Choices y_positions = new Choices("one", "two", "three", "four", "five", "six", "seven", "eight");

            //now build the complete pattern...
            GrammarBuilder commandRequest = new GrammarBuilder();
            commandRequest.Append(action);
            commandRequest.Append(pieces);
            commandRequest.Append(new Choices("at space", "to space"));
            commandRequest.Append(x_positions);
            commandRequest.Append(y_positions);

            //create the pizza grammar
            _grammar = new Grammar(commandRequest);
        }
    }
}
