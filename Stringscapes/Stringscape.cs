using DylanMonoGameIntro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;

namespace Stringscapes
{

    class Stringscape
    {
        List<Letter> letters;
        string currentWord;
        List<int> orderOfLetters;
        BaseCircle baseCircle;
        SpriteFont letterFont;
        private readonly SpriteFont definitionFont;
        readonly SpriteFont wordListFont;
        Texture2D letterTexture;
        GraphicsDevice GraphicsDevice;
        Sprite backdrop;
        Sprite leftArrow;
        Sprite rightArrow;
        Sprite upArrow;
        Sprite downArrow;
        string baseWord = "";
        public string chosenWord = "";
        Random gen = new Random();
        public int wordsStartPos;
        Rectangle MouseRect;
        MouseState previousState;
        readonly Vector2[] newShufflePositions;
        bool animateShuffle = false;
        float shuffleLerpCount = 0;
        float nextPageLerpCount = 0;
        int wordPage = 0;
        int wordPageLimit = 1;
        public List<CorrectWord> CorrectWords = new List<CorrectWord>();
        Dictionary<string, string[]> correctWordDefinitions;
        public Vector2 LongestWordSize;
        Vector2 wordBoxPosition;
        Vector2 previousWordBoxPosition;
        Vector2 startWordAnimationPosition;
        bool animateNextPage = false;
        Vector2 padding = new Vector2(10, 10);
        int wordPageSize = 0;
        readonly int definitionPageSize;
        HttpClient client;
        string displayedDefinition;
        int definedWordIndex = -1;
        int wordDefinitionIndex = 0;


        async Task<string[]> GetDef(string word)
        {
            string pull = await client.GetStringAsync($"https://od-api.oxforddictionaries.com:443/api/v1/entries/en/{word.ToLower()}").ConfigureAwait(false);
            var definitionObject = JsonConvert.DeserializeObject<WordDef>(pull);
            var x = definitionObject.Results.FirstOrDefault()?.lexicalEntries.FirstOrDefault()?.entries.FirstOrDefault()?.senses.FirstOrDefault();
            //var definitions = x.definitions != null && x.definitions.Length > 0 ? x.definitions : x.subsenses.FirstOrDefault()?.definitions;
            var definitions = new List<string>();
            if (definitionObject.Results != null)
            {
                foreach (var result in definitionObject.Results)
                {
                    if (result.lexicalEntries != null)
                    {
                        foreach (var lexicalEntry in result.lexicalEntries)
                        {
                            if (lexicalEntry.entries != null)
                            {
                                foreach (var entry in lexicalEntry.entries)
                                {
                                    if (entry.senses != null)
                                    {
                                        foreach (var sense in entry.senses)
                                        {
                                            if (sense.definitions != null && sense.definitions.Length > 0 && sense.definitions[0] != null)
                                            {
                                                definitions.Add("(" + lexicalEntry.lexicalCategory + ") " + sense.definitions[0]);
                                            }
                                            if (sense.subsenses != null)
                                            {
                                                foreach (var subsense in sense.subsenses)
                                                {
                                                    if (subsense.definitions != null && subsense.definitions.Length > 0 && subsense.definitions[0] != null)
                                                    {
                                                        definitions.Add("(" + lexicalEntry.lexicalCategory + ") " + subsense.definitions[0]);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return definitions.ToArray();
        }

        public void Reshuffle()
        {
            if (!animateShuffle)
            {
                List<int> positions = new List<int>();
                for (int i = 0; i < baseWord.Length; i++)
                {
                    positions.Add(i);
                }

                for (int i = 0; i < baseWord.Length; i++)
                {
                    int num = gen.Next(0, positions.Count);

                    newShufflePositions[i] = baseCircle.innerPoints[positions[num]] - new Vector2(letterTexture.Width / 2, letterTexture.Height / 2);
                    positions.RemoveAt(num);
                }

                animateShuffle = true;
            }
        }

        void ShuffleAnimation()
        {
            if (animateShuffle)
            {
                for (int i = 0; i < letters.Count; i++)
                {
                    letters[i].Position = Vector2.Lerp(letters[i].Position, newShufflePositions[i], shuffleLerpCount);
                }

                shuffleLerpCount += .06f;

                if (shuffleLerpCount >= 1)
                {
                    shuffleLerpCount = 0;
                    animateShuffle = false;
                }
            }
        }

        void NextPageAnimation()
        {
            if (animateNextPage)
            {
                wordBoxPosition = Vector2.SmoothStep(previousWordBoxPosition, startWordAnimationPosition, nextPageLerpCount);

                nextPageLerpCount += 0.05f;

                if (nextPageLerpCount >= 1)
                {
                    nextPageLerpCount = 0;
                    animateNextPage = false;
                }
            }
        }        

        public Stringscape(string word, Texture2D baseCircleTexture, Texture2D letterTexture, Texture2D leftRightArrowTexture, Texture2D upDownArrowTexture, GraphicsDevice GraphicsDevice, SpriteFont letterFont, SpriteFont wordListFont, SpriteFont definitionFont)
        {
            word = word.ToUpper();
            baseWord = word;
            newShufflePositions = new Vector2[baseWord.Length];
            this.letterFont = letterFont;
            this.definitionFont = definitionFont;
            baseCircle = new BaseCircle(baseCircleTexture, new Vector2(0, GraphicsDevice.Viewport.Height - baseCircleTexture.Height), new Color(Color.LightSteelBlue, 235), GraphicsDevice, letterTexture.Width / 2, word.Length);
            letters = new List<Letter>();
            this.letterTexture = letterTexture;
            this.GraphicsDevice = GraphicsDevice;
            Texture2D backdropPixel = new Texture2D(GraphicsDevice, 1, 1);
            backdropPixel.SetData(new[] { Color.White });
            backdrop = new Sprite(backdropPixel, Vector2.Zero, Color.CornflowerBlue, GraphicsDevice)
            {
                Scale = new Vector2(baseCircleTexture.Width, GraphicsDevice.Viewport.Height)
            };
            for (int i = 0; i < word.Length; i++)
            {
                letters.Add(new Letter(letterTexture, baseCircle.innerPoints[i] - new Vector2(letterTexture.Width / 2, letterTexture.Height / 2), Color.TransparentBlack, word[i], letterFont, GraphicsDevice, Color.OrangeRed));
            }
            Reshuffle();
            currentWord = "";
            orderOfLetters = new List<int>();
            wordsStartPos = 850;
            definitionPageSize = GraphicsDevice.Viewport.Width - 1050;
            this.wordListFont = wordListFont;
            LongestWordSize = wordListFont.MeasureString("DDDDDDDD");
            leftArrow = new Sprite(leftRightArrowTexture, new Vector2(900, 600), Color.White, GraphicsDevice);
            rightArrow = new Sprite(leftRightArrowTexture, new Vector2(GraphicsDevice.Viewport.Width - leftRightArrowTexture.Width - 50, 600), Color.White, GraphicsDevice)
            {
                SpriteEffects = SpriteEffects.FlipHorizontally
            };
            upArrow = new Sprite(upDownArrowTexture, new Vector2(900, 730), Color.Black, GraphicsDevice);
            downArrow = new Sprite(upDownArrowTexture, new Vector2(900, 730 + upDownArrowTexture.Height + 20), Color.Black, GraphicsDevice)
            {
                SpriteEffects = SpriteEffects.FlipVertically
            };

            client = new HttpClient();
            client.DefaultRequestHeaders.Add("app_id", "e8e7ecee");
            client.DefaultRequestHeaders.Add("app_key", "610cfa0b0947d48788edd521c5542dfb");
            displayedDefinition = "";
            correctWordDefinitions = new Dictionary<string, string[]>();
        }

        public void Update(MouseState mouse)
        {
            MouseRect = new Rectangle(mouse.X, mouse.Y, 1, 1);
            for (int i = 0; i < letters.Count; i++)
            {
                letters[i].Update(mouse);
            }

            ShuffleAnimation();
            
            if (!animateNextPage)
            {
                wordBoxPosition = new Vector2(wordsStartPos - ((padding.X + wordPageSize) * wordPage - 10), 50);

                //if (previousWordBoxPosition != wordBoxPosition && previousWordBoxPosition != Vector2.Zero)
                //{
                //    animateNextPage = true;
                //    startWordAnimationPosition = wordBoxPosition;
                //}
                previousWordBoxPosition = wordBoxPosition;
            }
            // NextPageAnimation();

            bool firstClickCheck = false;
            bool isNoneClicked = true;
            for (int i = 0; i < letters.Count; i++)
            {
                if (letters[i].isClicked && orderOfLetters.Count == 0)
                {
                    firstClickCheck = true;
                }
                if (letters[i].Color != Color.TransparentBlack)
                {
                    isNoneClicked = false;
                }
                for (int t = 0; t < letters.Count; t++)
                {
                    if (letters[i].isClicked && !letters[t].didPass && !letters[i].didPass && i != t && letters[t].mouseHoveringOver)
                    {
                        letters[i].isClicked = false;
                        letters[i].didPass = true;
                        letters[i].connectingLetterPos = letters[t].Position + new Vector2(letters[t].Radius, letters[t].Radius);
                        orderOfLetters.Add(t);
                    }
                    if (letters[t].isClicked || letters[t].didPass)
                    {
                        if (t != i)
                        {
                            firstClickCheck = false;
                        }
                    }
                }
                if (firstClickCheck)
                {
                    orderOfLetters.Add(i);
                    firstClickCheck = false;
                }
            }
            if (isNoneClicked)
            {
                chosenWord = currentWord;
                orderOfLetters.Clear();
                isNoneClicked = false;
            }
            if (orderOfLetters.Count >= 2)
            {
                if (letters[orderOfLetters[orderOfLetters.Count - 2]].mouseHoveringOver)
                {
                    letters[orderOfLetters[orderOfLetters.Count - 1]].isClicked = false;
                    letters[orderOfLetters[orderOfLetters.Count - 1]].didPass = false;
                    letters[orderOfLetters[orderOfLetters.Count - 1]].Update(mouse);
                    letters[orderOfLetters[orderOfLetters.Count - 2]].isClicked = true;
                    letters[orderOfLetters[orderOfLetters.Count - 2]].didPass = false;
                    orderOfLetters.RemoveAt(orderOfLetters.Count - 1);
                }
            }
            currentWord = "";
            for (int i = 0; i < orderOfLetters.Count; i++)
            {
                if (letters[orderOfLetters[i]].didPass || letters[orderOfLetters[i]].isClicked)
                {
                    currentWord += letters[orderOfLetters[i]].letter;
                }
            }

            if(CorrectWords.Count >  wordPageLimit*18)
            {
                wordPageLimit++;
            }
            if (mouse.LeftButton == ButtonState.Pressed && previousState.LeftButton == ButtonState.Released)
            {
                if (leftArrow.Bounds.Intersects(MouseRect) && wordPage != 0 )
                {
                    wordPage--;
                }
                else if (rightArrow.Bounds.Intersects(MouseRect) && wordPage < wordPageLimit - 1)
                {
                    wordPage++;
                }
                else if (displayedDefinition != "")
                {
                    if (upArrow.Bounds.Intersects(MouseRect) && wordDefinitionIndex > 0)
                    {
                        wordDefinitionIndex--;
                    }
                    else if (downArrow.Bounds.Intersects(MouseRect) && wordDefinitionIndex < correctWordDefinitions[CorrectWords[definedWordIndex].word].Length - 1)
                    {
                        wordDefinitionIndex++;
                    }
                }
                else
                {
                    upArrow.Color = Color.Black;
                    downArrow.Color = Color.Black;
                }

            }
            if (wordDefinitionIndex == 0 && displayedDefinition != "")
            {
                upArrow.Color = Color.DarkSlateGray;
                downArrow.Color = Color.Black;
            }
            else if (displayedDefinition != "" && wordDefinitionIndex == correctWordDefinitions[CorrectWords[definedWordIndex].word].Length - 1)
            {
                downArrow.Color = Color.DarkSlateGray;
                upArrow.Color = Color.Black;
            }            
            else
            {
                upArrow.Color = Color.Black;
                downArrow.Color = Color.Black;
            }
            if(displayedDefinition != "" && correctWordDefinitions[CorrectWords[definedWordIndex].word].Length == 1)
            {
                upArrow.Color = Color.DarkSlateGray;
                downArrow.Color = Color.DarkSlateGray;
            }


            for (int i = 0; i < CorrectWords.Count; i++)
            {
                if (MouseRect.Intersects(CorrectWords[i].Bounds) && mouse.LeftButton == ButtonState.Pressed && previousState.LeftButton == ButtonState.Released && definedWordIndex < 0 && CorrectWords[i].Color != Color.Yellow && CorrectWords[i].Color != Color.Red)
                {
                    CorrectWords[i].Color = Color.Yellow;
                    if (!correctWordDefinitions.ContainsKey(CorrectWords[i].word))
                    {
                        var x = GetDef(CorrectWords[i].word);
                        try
                        {
                            x.Wait();
                            if (x.IsCompleted)
                            {
                                wordDefinitionIndex = 0;
                                correctWordDefinitions.Add(CorrectWords[i].word, x.Result);
                                displayedDefinition = correctWordDefinitions[CorrectWords[i].word][wordDefinitionIndex];

                                CorrectWords[i].Color = Color.DarkGreen;
                            }
                            else if (x.IsFaulted)
                            {
                                CorrectWords[i].Color = Color.Purple;
                            }
                        }
                        catch
                        {
                            CorrectWords[i].Color = Color.Red;
                            correctWordDefinitions.Add(CorrectWords[i].word, new string[1] { "" });
                        }
                    }
                    if (correctWordDefinitions[CorrectWords[i].word][0] == "")
                    {
                        CorrectWords[i].Color = Color.Red;
                        displayedDefinition = "";
                    }
                    else
                    {
                        wordDefinitionIndex = 0;
                        CorrectWords[i].Color = Color.DarkGreen;
                        displayedDefinition = correctWordDefinitions[CorrectWords[i].word][wordDefinitionIndex];
                    }

                    definedWordIndex = i;
                }
            }
            if (definedWordIndex >= 0 && !MouseRect.Intersects(CorrectWords[definedWordIndex].Bounds) && !MouseRect.Intersects(upArrow.Bounds) && !MouseRect.Intersects(downArrow.Bounds) && mouse.LeftButton == ButtonState.Pressed)
            {
                displayedDefinition = "";
                CorrectWords[definedWordIndex].Color = Color.Black;
                definedWordIndex = -1;
            }

            previousState = mouse;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            wordPageSize = GraphicsDevice.Viewport.Width - wordsStartPos;
            int rowsPerColumn = 6;

            for (int col = 0; col < CorrectWords.Count; col += rowsPerColumn)
            {
                for (int row = 0; row < rowsPerColumn; row++)
                {
                    int index = col + row;
                    if (index >= CorrectWords.Count) { break; }

                    CorrectWords[index].UpdatePosition(wordBoxPosition + new Vector2(col / rowsPerColumn * (LongestWordSize.X + padding.X), row * (LongestWordSize.Y + padding.Y)));
                    CorrectWords[index].Draw(spriteBatch, GraphicsDevice);
                }
            }
            string pageLabel = "Page " + (wordPage + 1).ToString();
            spriteBatch.DrawString(wordListFont, pageLabel, new Vector2((rightArrow.Position.X + rightArrow.Image.Width + leftArrow.Position.X) / 2 - (wordListFont.MeasureString(pageLabel).X /2), leftArrow.Position.Y), Color.Black);

            if (displayedDefinition != "")
            {
                displayedDefinition = correctWordDefinitions[CorrectWords[definedWordIndex].word][wordDefinitionIndex];

                if (displayedDefinition.Contains("°"))
                {
                    int indexOfDegree = displayedDefinition.IndexOf('°');
                    if (displayedDefinition[indexOfDegree + 1] == 'F')
                    {
                        displayedDefinition = displayedDefinition.Remove(indexOfDegree + 1, 1);
                        displayedDefinition = displayedDefinition.Insert(indexOfDegree + 1, " Fahrenheit");
                    }
                    else if (displayedDefinition[indexOfDegree + 1] == 'C')
                    {
                        displayedDefinition = displayedDefinition.Remove(indexOfDegree + 1, 1);
                        displayedDefinition = displayedDefinition.Insert(indexOfDegree + 1, " Celsius");
                    }
                    displayedDefinition = displayedDefinition.Replace("°", " degrees");
                }

                string[] wordsInDefinition = displayedDefinition.Split(' ');
                string lineString = "";
                int line = 0;
                for (int i = 0; i < wordsInDefinition.Length; i++)
                {
                    if (definitionFont.MeasureString(lineString + wordsInDefinition[i]).X <= definitionPageSize)
                    {
                        lineString += wordsInDefinition[i] + " ";
                    }
                    else
                    {
                        spriteBatch.DrawString(definitionFont, lineString, new Vector2(1050, 750 + line * 55), Color.Black);
                        lineString = wordsInDefinition[i] + " ";
                        line++;
                    }
                }
                spriteBatch.DrawString(definitionFont, lineString, new Vector2(1050, 750 + line * 55), Color.Black);
            }

            backdrop.Draw(spriteBatch);
            baseCircle.Draw(spriteBatch);

            for (int i = 0; i < letters.Count; i++)
            {
                letters[i].DrawLine(spriteBatch);
            }
            for (int i = 0; i < letters.Count; i++)
            {
                letters[i].Draw(spriteBatch);
            }

            Vector2 sizeOfText = letterFont.MeasureString(currentWord);
            spriteBatch.DrawString(letterFont, currentWord, baseCircle.Position + new Vector2(baseCircle.Radius, -60) - (sizeOfText / 2), Color.Black);

            leftArrow.Draw(spriteBatch);
            rightArrow.Draw(spriteBatch);
            upArrow.Draw(spriteBatch);
            downArrow.Draw(spriteBatch);
        }
    }
}