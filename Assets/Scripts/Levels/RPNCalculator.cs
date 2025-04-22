using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class RPNCalculator
{
    public static float Evaluate(string expression, Dictionary<string, float> variables)
    {
        var stack = new Stack<float>();
        var tokens = expression.Split(' ');

        foreach (var token in tokens)
        {
            if (float.TryParse(token, out float number))
            {
                stack.Push(number);
            }
            else if (variables.ContainsKey(token))
            {
                stack.Push(variables[token]);
            }
            else
            {
                float b = stack.Pop();
                float a = stack.Pop();

                switch (token)
                {
                    case "+": stack.Push(a + b); break;
                    case "-": stack.Push(a - b); break;
                    case "*": stack.Push(a * b); break;
                    case "/": stack.Push(a / b); break;
                    case "%": stack.Push(a % b); break;
                    default: throw new System.ArgumentException($"Unknown operator: {token}");
                }
            }
        }

        return stack.Pop();
    }
    public static int CalculateEnemyCount(string rpnExpression, int wave)
    {
        try
        {
            var variables = new Dictionary<string, float>
            {
                { "wave", wave }     
            };

            float result = Evaluate(rpnExpression, variables);
            return Mathf.Max(1, Mathf.FloorToInt(result)); 
        }
        catch (System.Exception e)
        {
            
            return 5; 
        }
    }

    public static int CalculateEnemyCount(string rpnExpression, int wave, int baseHp)
    {
        try
        {
            var variables = new Dictionary<string, float>
            {
                { "base", baseHp },  
                { "wave", wave }     
            };

            float result = Evaluate(rpnExpression, variables);
            return Mathf.Max(1, Mathf.FloorToInt(result));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"HPcalculate fail: {e.Message} ( {rpnExpression})");
            return baseHp; // return base
        }
    }

}