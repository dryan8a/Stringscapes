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
        readonly GraphicsDevice GraphicsDevice;
        public Sprite backdrop;
        string baseWord = "";
        public string chosenWord = "";
        Random gen = new Random();
        Rectangle MouseRect;
        MouseState previousState;
        public WordManager wordManager;
        readonly Vector2[] newShufflePositions;
        bool animateShuffle = false;
        float shuffleLerpCount = 0;

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
            this.wordListFont = wordListFont;

            wordManager = new WordManager(leftRightArrowTexture, upDownArrowTexture, GraphicsDevice, wordListFont, definitionFont);
        }

        public void Update(MouseState mouse)
        {
            MouseRect = new Rectangle(mouse.X, mouse.Y, 1, 1);
            for (int i = 0; i < letters.Count; i++)
            {
                letters[i].Update(mouse);
            }

            ShuffleAnimation();
                    
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

            wordManager.Update(mouse, previousState);

            previousState = mouse;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            backdrop.Draw(spriteBatch);
            baseCircle.Draw(spriteBatch);

            wordManager.Draw(spriteBatch);

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
        }
    }
}