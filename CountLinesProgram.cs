using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 代码行数统计
{
    class CountLinesProgram
    {
        private enum OutputLevel { 无显示, 极少, 简略, 详细, 完全 };
        private readonly OutputLevel priority;
        private readonly List<String> extensions;
        private int[] everyExtensionLines;

        public CountLinesProgram()
        {
            this.extensions = new List<string>(Input("请输入合法拓展名(要加.)，使用;号隔开：").Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            int priority = int.Parse(Input("输出等级(无显示=0, 极少=1, 简略=2, 详细=3, 完全=4)："));
            this.priority = (OutputLevel)priority;
            everyExtensionLines = new int[this.extensions.Count];
        }

        private bool CanPrint(OutputLevel messageLevel)
        {
            return (int)messageLevel <= (int)priority;
        }

        private String Input(string info = "")
        {
            if (info != "")
                Console.Write(info);
            String input = Console.ReadLine();
            while(input.EndsWith("..."))
                input = input.Substring(0, input.Length - 3) + Console.ReadLine();
            return input;
        }

        private int GetLinesCount(DirectoryInfo rootDirectory, List<string> extensions)
        {
            int linesCount = 0;
            try
            {
                foreach (var file in rootDirectory.GetFiles())
                {
                    int extensionIndex = extensions.IndexOf(file.Extension);
                    if (extensionIndex != -1) 
                    {
                        int count = File.ReadAllLines(file.FullName).Length;
                        this.everyExtensionLines[extensionIndex] += count;
                        linesCount += count;
                        if (CanPrint(OutputLevel.完全))
                            Console.WriteLine("文件" + file.Name + "包含代码 " + count + " 行");
                    }
                }
                foreach (var directory in rootDirectory.GetDirectories())
                {
                    string[] avoidFolders = new string[] { "out", "bin", "obj" };
                    if (avoidFolders.Contains(directory.Name))
                        continue;
                    int line = GetLinesCount(directory, extensions);
                    if (CanPrint(OutputLevel.详细))
                        Console.WriteLine("文件夹" + directory + "包含代码 " + line + " 行");
                    linesCount += line;
                }
            }
            catch (PathTooLongException) { }
            return linesCount;
        }

        public void Run()
        {
            List<string> rootDirectories = new List<string>(Input("拖入要统计的文件夹（支持拖入多个，要加\"）：").Trim().Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries));
            int[] linesCountOfTheseDirectoroies = new int[rootDirectories.Count];
            int sum = 0;
            for (var i = 0; i < rootDirectories.Count; i++)
            {
                linesCountOfTheseDirectoroies[i] = GetLinesCount(new DirectoryInfo(rootDirectories[i]), extensions);
                sum += linesCountOfTheseDirectoroies[i];
                if(CanPrint(OutputLevel.简略))
                    Console.WriteLine("文件夹" + rootDirectories[i] + "包含代码 " + linesCountOfTheseDirectoroies[i] + " 行");
            }
            if (CanPrint(OutputLevel.简略))
            {
                Console.WriteLine("代码统计结果：");
                for (int i = 0; i < this.extensions.Count; i++)
                {
                    Console.Write(extensions[i] + ": " + everyExtensionLines[i] + " 行");
                    Console.WriteLine(" ({0:0.0}%)", 100.0 * everyExtensionLines[i] / sum);
                }
            }
            if (CanPrint(OutputLevel.极少)) 
                Console.WriteLine("总计 " + sum + " 行代码");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            var program = new CountLinesProgram();
            while (true)
                program.Run();
        }
    }
}
