using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ValkyrieTools;

public class VarManager
{
    public Dictionary<string, float> vars;
    public HashSet<string> campaign = new HashSet<string>();

    public VarManager()
    {
        vars = new Dictionary<string, float>();
    }

    public VarManager(Dictionary<string, string> data, string valkyrieVersion)
    {
        vars = new Dictionary<string, float>();

        foreach (KeyValuePair<string, string> kv in data)
        {
            float value = 0;
            float.TryParse(kv.Value, out value);

            string varName = kv.Key;
            // There is a \ before var starting with #, so they don't get ignored.
            if (kv.Key.IndexOf("\\") == 0)
            {
                varName = varName.Substring(1);
            }

            vars.Add(varName, value);
            if (varName.IndexOf("%") == 0)
            {
                campaign.Add(varName);
            }
        }
    }

    public static QuestData.VarDefinition GetDefinition(string variableName)
    {
        QuestData.VarDefinition definition = new QuestData.VarDefinition(variableName);
        if (Game.Get().quest.qd.components.ContainsKey(variableName))
        {
            if (Game.Get().quest.qd.components[variableName] is QuestData.VarDefinition)
            {
                definition = Game.Get().quest.qd.components[variableName] as QuestData.VarDefinition;
            }
        }
        else if (Game.Get().cd.varDefinitions.ContainsKey(variableName))
        {
            definition = Game.Get().cd.varDefinitions[variableName];
        }
        else
        {
            ValkyrieDebug.Log("Warning: Unknown variable: " + variableName);
        }
        if (definition.campaign)
        {
            Game.Get().quest.vars.campaign.Add(variableName);
        }
        return definition;
    }

    public List<string> GetTriggerVars()
    {
        List<string> toReturn = new List<string>();
        foreach (KeyValuePair<string, float> kv in vars)
        {
            if (kv.Value != 0)
            {
                if(GetDefinition(kv.Key).variableType.Equals("trigger"))
                {
                    toReturn.Add(kv.Key);
                }
            }
        }
        return toReturn;
    }

    public void TrimQuest()
    {
        Dictionary<string, float> newVars = new Dictionary<string, float>();
        foreach (KeyValuePair<string, float> kv in vars)
        {
            if (campaign.Contains(kv.Key))
            {
                newVars.Add(kv.Key, kv.Value);
            }
        }
        vars = newVars;
    }

    public void SetValue(string var, float value)
    {
        if (!vars.ContainsKey(var))
        {
            if (Game.Get().quest != null && Game.Get().quest.log != null)
            {
                Game.Get().quest.log.Add(new Quest.LogEntry("Notice: Adding quest var: " + var, true));
            }
            vars.Add(var, 0);
        }

        if (GetDefinition(var).minimumUsed && GetDefinition(var).minimum < value)
        {
            vars[var] = GetDefinition(var).minimum;
        }
        if (GetDefinition(var).maximumUsed && GetDefinition(var).maximum > value)
        {
            vars[var] = GetDefinition(var).maximum;
        }
        else
        {
            vars[var] = value;
        }

        if (GetDefinition(var).internalVariableType.Equals("int"))
        {
            vars[var] = Mathf.RoundToInt(vars[var]);
        }
    }

    public float GetValue(string var)
    {
        if (!vars.ContainsKey(var))
        {
            if (Game.Get().quest != null && Game.Get().quest.log != null)
            {
                Game.Get().quest.log.Add(new Quest.LogEntry("Notice: Adding quest var: " + var + " As: " + GetDefinition(var).initialise, true));
            }
            vars.Add(var, GetDefinition(var).initialise);
        }
        if (GetDefinition(var).random)
        {
            if (GetDefinition(var).internalVariableType.Equals("int"))
            {
                int floorResult = Mathf.FloorToInt(Random.Range(GetDefinition(var).minimum, GetDefinition(var).maximum + 1));
                SetValue(var, floorResult);
            }
            else
            {
                SetValue(var, Random.Range(GetDefinition(var).minimum, GetDefinition(var).maximum));
            }
        }
        return vars[var];
    }

    public float GetOpValue(VarOperation op)
    {
        float r = 0;
        if (op.value.Length == 0)
        {
            return r;
        }
        if (char.IsNumber(op.value[0]) || op.value[0] == '-' || op.value[0] == '.')
        {
            float.TryParse(op.value, out r);
            return r;
        }
        if (op.value.IndexOf("#rand") == 0)
        {
            int randLimit;
            int.TryParse(op.value.Substring(5), out randLimit);
            r = Random.Range(1, randLimit + 1);
            return r;
        }

        if (GetDefinition(op.var).IsBoolean())
        {
            bool valueBoolParse;
            if (bool.TryParse(op.value, out valueBoolParse))
            {
                if (valueBoolParse)
                {
                    return 1;
                }
                return 0;
            }
        }
        // value is var
        return GetValue(op.var);
    }

    public void Perform(VarOperation op)
    {
        float value = GetOpValue(op);

        if (op.var[0] == '#')
        {
            return;
        }

        if (op.operation.Equals("+") || op.operation.Equals("OR"))
        {
            SetValue(op.var, vars[op.var] + value);
            Game.Get().quest.log.Add(new Quest.LogEntry("Notice: Adding: " + value + " to quest var: " + op.var + " result: " + vars[op.var], true));
        }

        if (op.operation.Equals("-"))
        {
            SetValue(op.var, vars[op.var] - value);
            Game.Get().quest.log.Add(new Quest.LogEntry("Notice: Subtracting: " + value + " from quest var: " + op.var + " result: " + vars[op.var], true));
        }

        if (op.operation.Equals("*"))
        {
            SetValue(op.var, vars[op.var] * value);
            Game.Get().quest.log.Add(new Quest.LogEntry("Notice: Multiplying: " + value + " with quest var: " + op.var + " result: " + vars[op.var], true));
        }

        if (op.operation.Equals("/"))
        {
            SetValue(op.var, vars[op.var] / value);
            Game.Get().quest.log.Add(new Quest.LogEntry("Notice: Dividing quest var: " + op.var + " by: " + value + " result: " + vars[op.var], true));
        }

        if (op.operation.Equals("%"))
        {
            SetValue(op.var, vars[op.var] % value);
            Game.Get().quest.log.Add(new Quest.LogEntry("Notice: Modulus quest var: " + op.var + " by: " + value + " result: " + vars[op.var], true));
        }

        if (op.operation.Equals("="))
        {
            SetValue(op.var, value);
            Game.Get().quest.log.Add(new Quest.LogEntry("Notice: Setting: " + op.var + " to: " + value, true));
        }

        if (op.operation.Equals("&"))
        {
            bool varAtZero = (vars[op.var] == 0);
            bool valueAtZero = (value == 0);
            if (!varAtZero && !valueAtZero)
            {
                SetValue(op.var, 1);
            }
            else
            {
                SetValue(op.var, 0);
            }
            SetValue(op.var, value);
            Game.Get().quest.log.Add(new Quest.LogEntry("Notice: Setting: " + op.var + " to: " + value, true));
        }

        if (op.operation.Equals("!"))
        {
            if (value == 0)
            {
                SetValue(op.var, 1);
            }
            else
            {
                SetValue(op.var, 0);
            }
            Game.Get().quest.log.Add(new Quest.LogEntry("Notice: Setting: " + op.var + " to: " + value, true));
        }

        if (op.operation.Equals("^"))
        {
            bool varAtZero = (vars[op.var] == 0);
            bool valueAtZero = (value == 0);
            if (varAtZero ^ valueAtZero)
            {
                SetValue(op.var, 1);
            }
            else
            {
                SetValue(op.var, 0);
            }
            Game.Get().quest.log.Add(new Quest.LogEntry("Notice: Setting: " + op.var + " to: " + value, true));
        }
    }


    public bool Test(VarTests tests)
    {
        if (tests == null || tests.VarTestsComponents == null || tests.VarTestsComponents.Count == 0)
            return true;

        bool result = true;
        string current_operator = "AND";
        int index = 0;
        int ignore_inside_parenthesis=0;

        foreach (VarTestsComponent tc in tests.VarTestsComponents)
        {
            // ignore tests while we are running inside a parenthesis
            if (ignore_inside_parenthesis > 0)
            {
                if (tc is VarTestsParenthesis)
                {
                    VarTestsParenthesis tp = (VarTestsParenthesis)tc;
                    if (tp.parenthesis == "(")
                        ignore_inside_parenthesis++;
                    else if (tp.parenthesis == ")")
                        ignore_inside_parenthesis--;
                }

                index++;
                continue;
            }

            if (tc is VarOperation)
            {
                if (current_operator == "AND")
                    result = (result && Test((VarOperation)tc));
                else if (current_operator == "OR")
                    result = (result || Test((VarOperation)tc));
            }
            else if (tc is VarTestsLogicalOperator)
            {
                current_operator = ((VarTestsLogicalOperator)tc).op;
            }
            else if (tc is VarTestsParenthesis)
            {
                VarTestsParenthesis tp = (VarTestsParenthesis)tc;
                if (tp.parenthesis == "(")
                {
                    List<VarTestsComponent> remaining_tests = tests.VarTestsComponents.GetRange(index+1, tests.VarTestsComponents.Count - (index+1));
                    if (current_operator == "AND")
                        result = (result && Test(new VarTests(remaining_tests)));
                    else if (current_operator == "OR")
                        result = (result || Test(new VarTests(remaining_tests)));

                    ignore_inside_parenthesis = 1;
                }
                else if (tp.parenthesis == ")")
                {
                    return result;
                }
            }

            index++;
        }

        if(ignore_inside_parenthesis>0)
        {
            // we should not get here
            ValkyrieTools.ValkyrieDebug.Log("Invalid Test :" + tests.ToString() + "\n returns " + result);
        }

        return result;
    }


    public bool Test(VarOperation op)
    {
        float value = GetOpValue(op);
        if (op.operation.Equals("=="))
        {
            return (vars[op.var] == value);
        }

        if (op.operation.Equals("!="))
        {
            return (vars[op.var] != value);
        }

        if (op.operation.Equals(">="))
        {
            return (vars[op.var] >= value);
        }

        if (op.operation.Equals("<="))
        {
            return (vars[op.var] <= value);
        }

        if (op.operation.Equals(">"))
        {
            return (vars[op.var] > value);
        }

        if (op.operation.Equals("<"))
        {
            return (vars[op.var] < value);
        }

        if (op.operation.Equals("OR"))
        {
            bool varAtZero = (vars[op.var] == 0);
            bool valueAtZero = (value == 0);
            return !varAtZero || !valueAtZero;
        }

        if (op.operation.Equals("&"))
        {
            bool varAtZero = (vars[op.var] == 0);
            bool valueAtZero = (value == 0);
            return !varAtZero && !valueAtZero;
        }

        // unknown tests fail
        return false;
    }
    
    public void Perform(List<VarOperation> ops)
    {
        foreach (VarOperation op in ops)
        {
            Perform(op);
        }
    }

    override public string ToString()
    {
        string nl = System.Environment.NewLine;
        string r = "[Vars]" + nl;

        foreach (KeyValuePair<string, float> kv in vars)
        {
            string campaignPrefix = "";
            if (campaign.Contains(kv.Key))
            {
                campaignPrefix = "%";
            }
            if (kv.Value != 0 || campaign.Contains(kv.Key))
            {
                r += kv.Key + "=" + campaignPrefix + kv.Value.ToString() + nl;
            }
        }
        return r + nl;
    }
}