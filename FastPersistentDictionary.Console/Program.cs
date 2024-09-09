using FastPersistentDictionary;
using System.Diagnostics;

internal class Program
{
    static void Main(string[] args)
    {
        string strValueTest = "Star Wars (later retitled Star Wars: Episode IV – A New Hope) is a 1977 American epic space opera film written and directed by George Lucas, produced by Lucasfilm and distributed by Twentieth Century-Fox. It is the first film released in the Star Wars film series and the fourth chronological chapter of the \"Skywalker Saga\". Set \"a long time ago\" in a fictional galaxy ruled by the tyrannical Galactic Empire, the story follows a group of freedom fighters known as the Rebel Alliance, who aim to destroy the Empire's newest weapon, the Death Star. When the Rebel leader Princess Leia is abducted by the Empire, Luke Skywalker acquires stolen architectural plans of the Death Star and sets out to rescue her while learning the ways of a metaphysical power known as \"the Force\" from the Jedi Master Obi-Wan Kenobi. The cast includes Mark Hamill, Harrison Ford, Carrie Fisher, Peter Cushing, Alec Guinness, Anthony Daniels, Kenny Baker, Peter Mayhew, David Prowse, and James Earl Jones.\r\n\r\nLucas had the idea for a science fiction film in the vein of Flash Gordon around the time he completed his first film, THX 1138 (1971), and he began working on a treatment after the release of American Graffiti (1973). After numerous rewrites, filming took place throughout 1975 and 1976 in locations including Tunisia and Elstree Studios in Hertfordshire, England. Lucas formed the visual effects company Industrial Light & Magic to help create the film's visual effects. Star Wars suffered production difficulties: the cast and crew believed the film would be a failure, and it went $3 million over budget due to delays.\r\n\r\nFew were confident in the film's box office prospects. It was released in a small number of theaters in the United States on May 25, 1977, and quickly became a surprise blockbuster hit, leading to it being expanded to a much wider release. Star Wars opened to positive reviews, with praise for its special effects. It grossed $410 million worldwide during its initial run, surpassing Jaws (1975) to become the highest-grossing film until the release of E.T. the Extra-Terrestrial (1982); subsequent releases have brought its total gross to $775 million. When adjusted for inflation, Star Wars is the second-highest-grossing film in North America (behind Gone with the Wind) and the fourth-highest-grossing film of all time. It received Academy Awards, BAFTA Awards, and Saturn Awards, among others. The film has been reissued many times with Lucas's support—most significantly the 20th-anniversary theatrical \"Special Edition\"—and the reissues have contained many changes, including new scenes, visual effects, and dialogue.\r\n\r\nOften regarded as one of the greatest and most influential films ever made, Star Wars quickly became a pop culture phenomenon, launching an industry of tie-in products, including novels, comics, video games, amusement park attractions and merchandise such as toys, games, and clothing. It became one of the first 25 films selected by the United States Library of Congress for preservation in the National Film Registry in 1989, and its soundtrack was added to the U.S. National Recording Registry in 2004. The Empire Strikes Back (1980) and Return of the Jedi (1983) followed Star Wars, rounding out the original Star Wars trilogy. A prequel trilogy and a sequel trilogy have since been released, in addition to two standalone films and various television series.\r\n\r\nPlot\r\nAmid a galactic civil war, Rebel Alliance spies have stolen plans to the Death Star, a colossal space station built by the Galactic Empire that is capable of destroying entire planets. Princess Leia Organa of Alderaan, secretly a Rebel leader, has obtained the schematics, but her ship is intercepted and boarded by Imperial forces under the command of Darth Vader. Leia is taken prisoner, but the droids R2-D2 and C-3PO escape with the plans, crashing on the nearby planet of Tatooine.\r\n\r\nThe droids are captured by Jawa traders, who sell them to the moisture farmers Owen and Beru Lars and their nephew, Luke Skywalker. While Luke is cleaning R2-D2, he discovers a recording of Leia requesting help from a former ally named Obi-Wan Kenobi. R2-D2 goes missing, and while searching for him, Luke is attacked by Sand People. He is rescued by the elderly hermit Ben Kenobi, who soon reveals himself to be Obi-Wan. He tells Luke about his past as one of the Jedi Knights, former peacekeepers of the Galactic Republic, who drew mystical abilities from the Force but were hunted to near-extinction by the Empire. Luke learns that his father, also a Jedi, fought alongside Obi-Wan during the Clone Wars until Vader, Obi-Wan's former pupil, turned to the dark side of the Force and murdered him. Obi-Wan gives Luke his father's lightsaber, the signature weapon of the Jedi.";

        var fastPersistentDictionary = new FastPersistentDictionary<string, string>();
       
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 0; i < 100000; i++) 
        {
            fastPersistentDictionary.Add("name_" + i.ToString(), strValueTest);

        }
        for (int i = 0; i < 100000; i++)
        {
            fastPersistentDictionary.TryGetValue("name_" + i.ToString(), out var val);


        }


        for (int i = 0; i < 1000; i++)
        {
            fastPersistentDictionary.Remove("name_" + i.ToString());


        }

        for (int i = 100001; i < 100100; i++)
        {
            fastPersistentDictionary.Add("name_" + i.ToString(), strValueTest);


        }

        stopwatch.Stop();
        Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
        Console.ReadLine();
    }
}