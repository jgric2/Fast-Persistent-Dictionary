namespace FastPersistentDictionary.Tests;

public sealed partial class PersistentDictionaryProTests
{
    [TestFixture]
    public class IndexTests
    {
        [Test]
        public void Index_ReturnsDictionaryIndexedByKeys()
        {
            var dictionary = new FastPersistentDictionary<int, string>();
            dictionary.Add(1, "One");
            dictionary.Add(2, "Two");
            dictionary.Add(3, "Three");

            var indexedDictionary = dictionary.Index();

            Assert.AreEqual(3, indexedDictionary.Count);
            Assert.IsTrue(indexedDictionary.ContainsKey(1));
            Assert.IsTrue(indexedDictionary.ContainsKey(2));
            Assert.IsTrue(indexedDictionary.ContainsKey(3));
            Assert.AreEqual(("One", 0), indexedDictionary[1]);
            Assert.AreEqual(("Two", 1), indexedDictionary[2]);
            Assert.AreEqual(("Three", 2), indexedDictionary[3]);
        }

        [Test]
        public void Index_ReturnsEmptyDictionaryForEmptyPersistentDictionaryPro()
        {
            var dictionary = new FastPersistentDictionary<int, string>();

            var indexedDictionary = dictionary.Index();
            Assert.AreEqual(0, indexedDictionary.Count);
        }

        [Test]
        public void Index_ReturnsDictionaryIndexedByValues()
        {
            var dictionary = new FastPersistentDictionary<int, string>(useCompression: false);
            //dictionary.Add(1, "Star Wars is an American epic space opera[1] media franchise created by George Lucas, which began with the eponymous 1977 film[a] and quickly became a worldwide pop culture phenomenon. The franchise has been expanded into various films and other media, including television series, video games, novels, comic books, theme park attractions, and themed areas, comprising an all-encompassing fictional universe.[b] Star Wars is one of the highest-grossing media franchises of all time.\r\n\r\nThe original 1977 film, retroactively subtitled Episode IV: A New Hope, was followed by the sequels Episode V: The Empire Strikes Back (1980) and Episode VI: Return of the Jedi (1983), forming the original Star Wars trilogy. Lucas later returned to the series to write and direct a prequel trilogy, consisting of Episode I: The Phantom Menace (1999), Episode II: Attack of the Clones (2002), and Episode III: Revenge of the Sith (2005). In 2012, Lucas sold his production company to Disney, relinquishing his ownership of the franchise. This led to a sequel trilogy, consisting of Episode VII: The Force Awakens (2015), Episode VIII: The Last Jedi (2017), and Episode IX: The Rise of Skywalker (2019).\r\n\r\nAll nine films, collectively referred to as the \"Skywalker Saga\", were nominated for Academy Awards, with wins going to the first two releases. Together with the theatrical live action \"anthology\" films Rogue One (2016) and Solo (2018), the combined box office revenue of the films equated to over US$10 billion, making Star Wars the third-highest-grossing film franchise of all time.");
            //dictionary.Add(2, "The Star Wars franchise depicts the adventures of characters \"A long time ago in a galaxy far, far away\"[3] across multiple fictional eras, in which humans and many species of aliens (often humanoid) co-exist with robots (typically referred to in the films as 'droids'), which may be programmed for personal assistance or battle.[4] Space travel between planets is common due to lightspeed hyperspace technology.[5][6][7] The planets range from wealthy, planet-wide cities to deserts scarcely populated by primitive tribes. Virtually any Earth biome, along with many fictional ones, has its counterpart as a Star Wars planet which, in most cases, teem with sentient and non-sentient alien life.[8] The franchise also makes use of other astronomical objects such as asteroid fields and nebulae.[9][10] Spacecraft range from small starfighters to large capital ships, such as the Star Destroyers, as well as space stations such as the moon-sized Death Stars.[11][12][13] Telecommunication includes two-way audio and audiovisual screens, holographic projections and hyperspace transmission.[14]");
            //dictionary.Add(3, "The universe of Star Wars is generally similar to the real universe but its laws of physics are less strict allowing for more imaginative stories.[15] One result of that is a mystical power known as the Force which is described in the original film as \"an energy field created by all living things ... [that] binds the galaxy together\".[16] The field is depicted as a kind of pantheistic god.[17] Through training and meditation, those whom \"the Force is strong with\" exhibit various superpowers (such as telekinesis, precognition, telepathy, and manipulation of physical energy).[18] It is believed nothing is impossible for the Force.[19] The mentioned powers are wielded by two major knightly orders at conflict with each other: the Jedi, peacekeepers of the Galactic Republic who act on the light side of the Force through non-attachment and arbitration, and the Sith, who use the dark side by manipulating fear and aggression.[20][21] While Jedi Knights can be numerous, the Dark Lords of the Sith (or 'Darths') are intended to be limited to two: a master and their apprentice.[22]\r\n\r\nThe franchise is set against a backdrop of galactic conflict involving republics and empires, such as the evil Galactic Empire.[23] The Jedi and Sith prefer the use of a weapon called the lightsaber, a blade of plasma that can cut through virtually any surface and deflect energy bolts.[24] The rest of the population, as well as renegades and soldiers, use plasma-powered blaster firearms.[25] In the outer reaches of the galaxy, crime syndicates such as the Hutt cartel are dominant.[26] Bounty hunters are often employed by both gangsters and governments, while illicit activities include smuggling and slavery.[26]\r\n\r\nThe combination of science fiction and fantasy elements makes Star Wars a very universal franchise, capable of telling stories of various genres.[27]");

            dictionary.Add(1, "One");
            dictionary.Add(2, "Two");
            dictionary.Add(3, "Three");

            var indexedDictionary = dictionary.Index().ToDictionary(x => x.Value.value, x => x.Value.index);

            Assert.AreEqual(3, indexedDictionary.Count);
            Assert.IsTrue(indexedDictionary.ContainsKey("One"));
            Assert.IsTrue(indexedDictionary.ContainsKey("Two"));
            Assert.IsTrue(indexedDictionary.ContainsKey("Three"));
            Assert.AreEqual(0, indexedDictionary["One"]);
            Assert.AreEqual(1, indexedDictionary["Two"]);
            Assert.AreEqual(2, indexedDictionary["Three"]);
        }
    }
}