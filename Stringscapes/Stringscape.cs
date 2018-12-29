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
            var definitions = x.definitions != null && x.definitions.Length > 0 ? x.definitions : x.subsenses.FirstOrDefault()?.definitions;

            return definitions;
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
            upArrow = new Sprite(upDownArrowTexture, new Vector2(900, 750), Color.White, GraphicsDevice);
            downArrow = new Sprite(upDownArrowTexture, new Vector2(900, 750 + upDownArrowTexture.Height + 25), Color.White, GraphicsDevice)
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
                wordBoxPosition = new Vector2(wordsStartPos - (2 * padding.X + wordPageSize) * wordPage, 50);

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

            if (mouse.LeftButton == ButtonState.Pressed && previousState.LeftButton == ButtonState.Released)
            {
                if (leftArrow.Bounds.Intersects(MouseRect) && wordPage != 0)
                {
                    wordPage--;
                }
                else if (rightArrow.Bounds.Intersects(MouseRect))
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
                
            }

            for (int i = 0; i < CorrectWords.Count; i++)
            {
                if (MouseRect.Intersects(CorrectWords[i].Bounds) && mouse.LeftButton == ButtonState.Pressed && definedWordIndex < 0 && CorrectWords[i].Color != Color.Yellow && CorrectWords[i].Color != Color.Red)
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

            if (displayedDefinition != "")
            {
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