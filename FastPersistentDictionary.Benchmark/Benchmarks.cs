using BenchmarkDotNet.Attributes;
using Microsoft.Isam.Esent.Collections.Generic;

namespace FastPersistentDictionary.Benchmark
{
    public class Benchmarks
    {
        private FastPersistentDictionary<string, string> fastPersistentDictionary;
        private Dictionary<string, string> standardDictionary;
        private PersistentDictionary<string, string> esentPersistentDictionary;

        [Params(10_000, 100_000, 1_000_000)]//, 100_000, 1_000_000)]
        public int N;

        [GlobalSetup]
        public void GlobalSetup()
        {
            string directoryEsent = "C:\\Users\\Admin\\Documents\\Copiers\\New folder\\Esent";
            string fastPersistentDictionary = "C:\\Users\\Admin\\Documents\\Copiers\\New folder\\dict.tt";

            this.fastPersistentDictionary = new FastPersistentDictionary<string, string>(path: fastPersistentDictionary);
            standardDictionary = new Dictionary<string, string>();
            esentPersistentDictionary = new PersistentDictionary<string, string>(directoryEsent);
            esentPersistentDictionary.Clear();

            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                string value = $"Value{i} {longSentences[i % longSentences.Length]}";
                this.fastPersistentDictionary.Add(key, value);
                standardDictionary.Add(key, value);
                esentPersistentDictionary.Add(key, value);
            }
        }

        [IterationSetup(Target = nameof(FastPersistentDictionary_Add))]
        public void IterationSetupForAddPersistent()
        {
            fastPersistentDictionary.Clear();
        }

        [IterationSetup(Target = nameof(StandardDictionary_Add))]
        public void IterationSetupForAddStandard()
        {
            standardDictionary.Clear();
        }

        [IterationSetup(Target = nameof(EsentPersistentDictionary_Add))]
        public void IterationSetupForAddEsent()
        {
            esentPersistentDictionary.Clear();
        }

        [IterationSetup(Targets = new[] { nameof(FastPersistentDictionary_Get), nameof(FastPersistentDictionary_Remove) })]
        public void IterationSetupForGetOrRemovePersistent()
        {
            fastPersistentDictionary.Clear();
            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                string value = $"Value{i} {longSentences[i % longSentences.Length]}";
                fastPersistentDictionary.Add(key, value);
            }
        }

        [IterationSetup(Targets = new[] { nameof(StandardDictionary_Get), nameof(StandardDictionary_Remove) })]
        public void IterationSetupForGetOrRemoveStandard()
        {
            standardDictionary.Clear();
            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                string value = $"Value{i} {longSentences[i % longSentences.Length]}";
                standardDictionary.Add(key, value);
            }
        }

        [IterationSetup(Targets = new[] { nameof(EsentPersistentDictionary_Get), nameof(EsentPersistentDictionary_Remove) })]
        public void IterationSetupForGetOrRemoveEsent()
        {
            esentPersistentDictionary.Clear();
            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                string value = $"Value{i} {longSentences[i % longSentences.Length]}";
                esentPersistentDictionary[key] = value;
            }
        }

        [Benchmark]
        public void FastPersistentDictionary_Add()
        {
            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                string value = $"Value{i} {longSentences[i % longSentences.Length]}";
                fastPersistentDictionary.Add(key, value);
            }
            fastPersistentDictionary.CompactDatabaseFile();
        }

        [Benchmark]
        public void StandardDictionary_Add()
        {
            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                string value = $"Value{i} {longSentences[i % longSentences.Length]}";
                standardDictionary.Add(key, value);
            }
        }

        [Benchmark]
        public void EsentPersistentDictionary_Add()
        {
            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                string value = $"Value{i} {longSentences[i % longSentences.Length]}";
                esentPersistentDictionary.Add(key, value);
            }
        }

        [Benchmark]
        public void FastPersistentDictionary_Get()
        {
            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                string value = fastPersistentDictionary[key];
            }
        }

        [Benchmark]
        public void StandardDictionary_Get()
        {
            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                string value = standardDictionary[key];
            }
        }

        [Benchmark]
        public void EsentPersistentDictionary_Get()
        {
            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                string value = esentPersistentDictionary[key];
            }
        }

        [Benchmark]
        public void FastPersistentDictionary_Remove()
        {
            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                fastPersistentDictionary.Remove(key);
            }
        }

        [Benchmark]
        public void StandardDictionary_Remove()
        {
            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                standardDictionary.Remove(key);
            }
        }

        [Benchmark]
        public void EsentPersistentDictionary_Remove()
        {
            for (int i = 0; i < N; i++)
            {
                string key = $"Key{i}";
                esentPersistentDictionary.Remove(key);
            }
        }

        string[] longSentences = new string[]
        {
            "In an unprecedented turn of events, the local community rallied together to support small businesses, showcasing a remarkable spirit of unity and resilience that had not been seen in decades.",
            "The picturesque village nestled in the foothills of the mountain range was often described as a hidden gem, attracting tourists who were eager to escape the hustle and bustle of city life.",
            "Under the sprawling oak tree that had stood for centuries, the children listened intently to their grandfather's stories, each tale more fascinating than the last.",
            "As the sun set over the horizon, casting a golden hue across the landscape, the weary travelers found solace in the serene beauty of their surroundings.",
            "In a meticulously orchestrated plan, the team of scientists embarked on a groundbreaking mission to explore the uncharted depths of the ocean.",
            "With each passing day, the artist's masterpiece began to take shape, a testament to her unwavering dedication and unparalleled talent.",
            "Despite numerous challenges, the project was completed on time, a feat that many had deemed impossible at the outset.",
            "Standing at the edge of the cliff, he felt a surge of adrenaline as he prepared to take the leap, trusting that his parachute would deploy as intended.",
            "The aroma of freshly baked bread wafted through the air, drawing customers to the quaint bakery that had become a beloved fixture in the neighborhood.",
            "Through rigorous training and sheer determination, she had transformed herself from a novice into a formidable athlete, ready to compete at the highest level.",
            "In a heartfelt speech, the retiring professor reflected on his decades-long career, expressing gratitude for the students who had inspired him along the way.",
            "The ancient manuscript, filled with cryptic symbols and forgotten languages, held secrets that had eluded scholars for generations.",
            "As the CEO addressed the board of directors, he outlined a bold vision for the company's future, one that promised innovation and growth.",
            "Navigating the dense forest required both skill and intuition, as the path was often obscured by thick underbrush and towering trees.",
            "The state-of-the-art facility was designed to be both functional and aesthetically pleasing, setting a new standard in architectural excellence.",
            "Her eloquent speech captured the audience's attention, leaving a lasting impression on everyone who had the privilege to hear it.",
            "In the midst of the bustling city, the tranquil garden offered a refuge for those seeking a moment of peace and reflection.",
            "The innovative startup quickly gained traction, attracting investors who were eager to support its disruptive technology.",
            "His journey across the continent was filled with encounters that broadened his horizons and deepened his appreciation for diverse cultures.",
            "The sprawling estate, with its manicured lawns and opulent mansion, was a testament to the family's enduring legacy.",
            "The relentless pursuit of knowledge drove the scientist to push the boundaries of what was considered possible.",
            "In the aftermath of the storm, the community came together to rebuild, demonstrating a resilience and camaraderie that was truly inspiring.",
            "Her groundbreaking research challenged conventional wisdom, opening new avenues for exploration and discovery.",
            "The historical novel, rich with vivid descriptions and intricate plotlines, transported readers to a bygone era.",
            "Despite facing numerous setbacks, the entrepreneur's perseverance ultimately led to the creation of a successful enterprise.",
            "The charismatic leader's vision for the future resonated with many, igniting a movement that would leave a lasting impact.",
            "In the quiet hours of the morning, the artist found solace in her studio, where creativity flowed freely and boundaries ceased to exist.",
            "The grand ballroom, adorned with crystal chandeliers and lavish decorations, was the perfect setting for the evening's gala.",
            "Drawing inspiration from nature, the architect designed a structure that seamlessly blended with its surroundings.",
            "The seasoned detective pieced together the clues, unraveling a mystery that had perplexed many for years.",
            "The breathtaking view from the mountaintop was a reward well worth the arduous journey.",
            "In a rapidly evolving industry, staying ahead of the curve required constant innovation and adaptability."
        };
    }
}