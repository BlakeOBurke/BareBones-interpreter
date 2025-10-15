using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BareBones_interpreter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Interpret(new Dictionary<string, int>(), InFromFile("fibbo.txt"));
            Console.ReadLine();
        }


        static string[] InFromFile(string path)
        {
            string[] text = File.ReadAllLines(path);
            return text;
        }

        static int baseRange = -1;
        static void Interpret(Dictionary<string,int> vars, string[]exe)
        {
            Print(exe, -1,baseRange);
            Dump(vars);

            //in the case of nested while statements, I will use a stack to grab the line # of the most nested while loop first
            Stack<(int,string)> whiles = new Stack<(int,string)>();
            int pc = 0;

            while (pc != exe.Length)//interpret lines until the program runs out
            {
                //Console.Clear();
                if (exe[pc] == "" || exe[pc] == null || exe[pc][0] == '#')
                {
                    pc++;
                    continue;
                }


                Console.WriteLine();
                Dump(vars, false);

                Print(exe, pc,baseRange);

                string[] cur = exe[pc].Split(new char[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);//the entire current line, split by ' ' and ';'
                switch (cur[0])//the instruction is always at the start of the line, so index 0
                {
                    case "clear":
                        Clear(vars, cur[1]);
                        break;

                    case "incr":
                        Incr(vars, cur[1]);
                        break;

                    case "decr":
                        Decr(vars, cur[1]);
                        break;

                    case "while":
                        if(whiles.Count()==0 || whiles.Peek()!= (pc, cur[1]))
                        {
                            whiles.Push((pc, cur[1]));
                        }
                        break;

                    case "end":
                        string condition = whiles.Peek().Item2;
                        if (vars[condition] != 0)
                        {
                            pc = whiles.Peek().Item1;
                            //Print(exe, pc);
                            //pc++;
                            continue;
                        }
                        else
                        {
                            whiles.Pop();
                        }
                        break;

                    default:
                        throw new Exception("invalid operand");
                        pc++;
                        continue;
                }

                Dump(vars);
                pc++;
            }
        }

        static void Clear(Dictionary<string,int> vars,string var)
        {
            if (vars.ContainsKey(var))
            {
                vars[var] = 0;
                return;
            }
            vars.Add(var, 0);
        }

        static void Incr(Dictionary<string,int> vars,string var)
        {
            if (vars.ContainsKey(var))
            {
                vars[var] +=1;
                return;
            }
            vars.Add(var, 1);
        }

        static void Decr(Dictionary<string,int> vars,string var)
        {
            if (vars.ContainsKey(var))
            {
                vars[var] = vars[var]==0?0 : vars[var]-1;
                return;
            }
            vars.Add(var, 0);
        }

        /// <summary>
        /// prints all of the values of the variables at the current point in the program 
        /// </summary>
        /// <param name="vars">the dictionary used to store the varibles as the program is run</param>
        static void Dump(Dictionary<string, int> vars,bool pause = true)
        {
            foreach (var key in vars.Keys)
            {
                Console.Write($"[{key}:{vars[key]}]");
            }
            Console.WriteLine();
            //if(pause) Console.ReadKey();
        }

        /// <summary>
        /// by default, prints the entire program
        /// if a range is given, only prints exe[i+-range]
        /// </summary>
        /// <param name="exe">the string array corresponding to the BareBones program</param>
        /// <param name="pc">the index in exe of the current instruction</param>
        /// <param name="range">the amount of lines above and below pc to print</param>
        static void Print(string[] exe,int pc,int range = -1)
        {
            for (int i = 0; i < exe.Length; i++)
            {
                if (i == pc)
                {
                    Console.BackgroundColor= ConsoleColor.Green;
                    Console.Write(exe[i]);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine();
                }
                else if(range == -1 || (pc+range >= i && pc-range <= i))
                {
                    if (exe[i].Count() > 0 && exe[i][0] == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(exe[i]);
                        Console.ForegroundColor = ConsoleColor.White;

                    }
                    else
                    {
                        Console.WriteLine(exe[i]);
                    }
                }
            }
            
        }
    }
}
