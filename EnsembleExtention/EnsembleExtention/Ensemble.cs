using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using System.Reflection;
using System.IO;
using Jint.Native;

namespace JintinterfaceTest
{
    class Ensemble
    {
        Engine engine;

        JsValue Bind_CalculateVolition;
        JsValue Bind_GetAction;
        JsValue Bind_DoAction;
        JsValue Bind_GetActions;
        JsValue Bind_RunTriggerRules;
        JsValue Bind_SetupNextTimeStep;


        public Ensemble()
        {
            engine = new Jint.Engine();
            defineConsole(engine);
            initializeEnsemble(engine);
            LoadFiles(engine,
                "../../Data/schema.json",
                "../../Data/cast.json",
                "../../Data/triggerRules.json",
                "../../Data/volitionRules.json",
                "../../Data/actions.json",
                "../../Data/history.json");
            BindApi(engine);

        }



        private void BindApi(Engine engine)
        {
            Bind_CalculateVolition = engine.Execute(@"function BindCalculateVolition(cast){return ensemble.calculateVolition(cast);}").GetValue("BindCalculateVolition");
            Bind_GetAction = engine.Execute(@"function BindGetAction(initiator,respondant,calculatedVolitions,cast){return ensemble.getAction(initiator,respondant,calculatedVolitions,cast);}").GetValue("BindGetAction");
            Bind_DoAction = engine.Execute(@"function BindDoAction(boundAction){return ensemble.doAction(boundAction);}").GetValue("BindDoAction");
            Bind_GetActions = engine.Execute(@"function BindGetActions(initiator,respondant,calculatedVolitions,cast,numIntent,numActionsPerIntent,numActionsPerGroup){return ensemble.getActions(initiator,respondant,calculatedVolitions,cast,numIntent,numActionsPerIntent,numActionsPerGroup);}").GetValue("BindGetActions");
            Bind_RunTriggerRules = engine.Execute(@"function BindRunTriggerRules(cast){return ensemble.runTriggerRules(cast);}").GetValue("BindRunTriggerRules");
            Bind_SetupNextTimeStep = engine.Execute(@"function BindSetupTimeStep(timestep){return ensemble.setupNextTimeStep(timestep);}").GetValue("BindSetupTimeStep");

        }

        private void defineConsole(Engine engine)
        {
            engine.SetValue("CLog", new Action<object>(Console.WriteLine));
            engine.Execute(@"console = {log : function(obj){CLog(obj);}}");
        }

        private void initializeEnsemble(Engine engine)//file reading found at https://stackoverflow.com/questions/38431553/using-third-party-js-libraries-with-jint
        {

            const string path = "../../ensemble.js";

            Console.WriteLine(File.Exists(path));
            using (var reader = new StreamReader(path))
            {
                var code = reader.ReadToEnd();

                var result = engine.Execute(code).GetCompletionValue();
            }
            engine.Execute(@"ensemble.init();");
        }
        private void LoadFiles(Engine engine, string schema, string cast, string triggerRules, string volitionRules, string Actions, string history)
        {
            engine.Execute(@"
            function parse(JsonString){
                return JSON.parse(JsonString);
            }
            ");
            var parse = engine.GetValue("parse");
            //pass the strings to the engine and parse them inside
            if (File.Exists(schema))
            {
                Console.WriteLine("schema File found.");
                using (var reader = new StreamReader(schema))
                {
                    string jsonString = reader.ReadToEnd();
                    var jsonObj = parse.Invoke(jsonString);
                    engine.Execute(@"function loadSchema(jsonObj){ensemble.loadSocialStructure(jsonObj);}");
                    var loadSchema = engine.GetValue("loadSchema");
                    loadSchema.Invoke(jsonObj);

                }
            }
            if (File.Exists(cast))
            {
                Console.WriteLine("cast file found.");
                using (var reader = new StreamReader(cast))
                {
                    string jsonString = reader.ReadToEnd();
                    var jsonObj = parse.Invoke(jsonString);
                    engine.Execute(@"function loadCast(jsonObj){ensemble.addCharacters(jsonObj);}");
                    var loadCast = engine.GetValue("loadCast");
                    loadCast.Invoke(jsonObj);
                }
            }
            if (File.Exists(triggerRules))
            {
                Console.WriteLine("rules file found.");
                using (var reader = new StreamReader(triggerRules))
                {
                    string jsonString = reader.ReadToEnd();
                    var jsonObj = parse.Invoke(jsonString);
                    engine.Execute(@"function loadRules(jsonObj){ensemble.addRules(jsonObj);}");
                    var LoadRules = engine.GetValue("loadRules");
                    LoadRules.Invoke(jsonObj);
                }
            }
            if (File.Exists(volitionRules))
            {
                Console.WriteLine("rules file found.");
                using (var reader = new StreamReader(volitionRules))
                {
                    string jsonString = reader.ReadToEnd();
                    var jsonObj = parse.Invoke(jsonString);
                    engine.Execute(@"function loadRules(jsonObj){ensemble.addRules(jsonObj);}");
                    var LoadRules = engine.GetValue("loadRules");
                    LoadRules.Invoke(jsonObj);
                }
            }
            if (File.Exists(Actions))
            {
                Console.WriteLine("Actions file found.");
                using (var reader = new StreamReader(Actions))
                {
                    string jsonString = reader.ReadToEnd();
                    var jsonObj = parse.Invoke(jsonString);
                    engine.Execute(@"function loadActions(jsonObj){ensemble.addActions(jsonObj);}");
                    var LoadActions = engine.GetValue("loadActions");
                    LoadActions.Invoke(jsonObj);
                }
            }
            if (File.Exists(history))
            {
                Console.WriteLine("history file found.");
                using (var reader = new StreamReader(history))
                {
                    string jsonString = reader.ReadToEnd();
                    var jsonObj = parse.Invoke(jsonString);
                    engine.Execute(@"function loadHistory(jsonObj){ensemble.addHistory(jsonObj);}");
                    var LoadHistory = engine.GetValue("loadHistory");
                    LoadHistory.Invoke(jsonObj);
                }
            }
        }
        public JsValue GetCharacters()
        {
            return engine.Execute(@"ensemble.getCharacters()").GetCompletionValue();
        }
        public JsValue CalculateVolitions()//assumes the use of the internal cast of characters that ensemble uses
        {

            return engine.Execute(@"ensemble.calculateVolition(ensemble.getCharacters());").GetCompletionValue();
        }
        public JsValue CalculateVolitions(JsValue cast)
        {
            return Bind_CalculateVolition.Invoke(cast);
        }
        public JsValue GetAction(JsValue initiator, JsValue respondant, JsValue calculatedVolitions, JsValue cast)//Always check that the result is not undefined before using it.
        {
            return Bind_GetAction.Invoke(initiator, respondant, calculatedVolitions, cast);
        }
        public JsValue DoAction(JsValue boundAction)
        {
            return Bind_DoAction.Invoke(boundAction);
        }
        public JsValue GetActions(JsValue initiator, JsValue respondant, JsValue calculatedVolitions, JsValue cast, int numIntent, int numActionsPerIntent, int numActionsperGroup)
        {
            return Bind_GetActions.Invoke(initiator, respondant, calculatedVolitions, cast, numIntent, numActionsPerIntent, numActionsperGroup);
        }
        public JsValue runTriggerRules(JsValue cast)
        {
            return Bind_RunTriggerRules.Invoke(cast);
        }
        //ignored set and get, as they are not useful if we can't manipulate the data at the C# level
        public JsValue setupNextTimeStep()
        {
            return Bind_SetupNextTimeStep.Invoke();
        }
        public void DumpSocialRecord()
        {
            engine.Execute(@"ensemble.dumpSocialRecord()");
        }
    }
}

