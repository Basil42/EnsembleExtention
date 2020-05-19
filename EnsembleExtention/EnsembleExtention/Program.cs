using Jint.Native;
using Jint.Native.Object;
using Jint.Parser.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace JintinterfaceTest
{
    class Program
    {
        static void PrintTriggerEffects(JsValue change)
        {
            Console.WriteLine("\nTrigger rules fired: ");
            string output = change.AsObject().GetProperty("explanations").Value.ToString();
            string[] delimiter = { @"TRIGGER" };
            foreach (string str in output.Split(delimiter, StringSplitOptions.RemoveEmptyEntries))
            {
                Console.WriteLine(str);
            }
        }

        static void Main(string[] args)
        {
            Ensemble ensemble = new Ensemble();
            var characters = ensemble.GetCharacters().AsArray();
            while (true)
            {
                Console.WriteLine("\n\nRunning timestep: " + ensemble.setupNextTimeStep());
                var change = ensemble.runTriggerRules(characters);
                PrintTriggerEffects(change);
                var vols = ensemble.CalculateVolitions();


                foreach (var initiator in characters.GetOwnProperties().Where(p => p.Key != "length"))
                {
                    
                    Console.WriteLine("\nPicking action for: " + initiator.Value.Value);
                    List<JsValue> actions = new List<JsValue>();
                    foreach (var target in characters.GetOwnProperties().Where(p => p.Key != "length"))
                    {
                        Console.Write("Towards " + target.Value.Value + ": ");

                        var action = ensemble.GetAction(initiator.Value.Value, target.Value.Value, vols, ensemble.GetCharacters(),5);
                        if(initiator.Value.Value.AsString() ==  "bob" && target.Value.Value.AsString() == "bob")
                        {
                            var bobActions = ensemble.GetActions(initiator.Value.Value, target.Value.Value, vols, characters, 1, 2, 2).AsArray();
                            Console.WriteLine("bob considered " + bobActions.GetLength() + " actions:");
                            for(int i =0; i < bobActions.GetLength(); i++)
                            {
                                var bobaction = bobActions.Get(i.ToString()).AsObject();
                                Console.WriteLine(bobaction.Get("name").AsString() + " with weight of" + bobaction.Get("weight").AsNumber());
                            }

                        }
                        Console.WriteLine(action.IsUndefined() ? action.ToString() : action.AsObject().GetProperty("name").Value + " with " + action.AsObject().GetProperty("weight").Value);
                        if (!action.IsUndefined())
                        {
                            actions.Add(action);
                        }
                    }
                    actions.Sort((lhs, rhs) => rhs.AsObject().GetProperty("weight").Value.AsNumber().CompareTo(lhs.AsObject().GetProperty("weight").Value.AsNumber()));
                    if (actions.Count > 0)
                    {
                        ensemble.DoAction(actions[0]);
                        vols = ensemble.CalculateVolitions();
                    }
                }

                Console.ReadLine();

            }
        }


    }
}
