﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stringscapes
{
    class BaseGameObject
    {
        List<Letter> letters;
        string currentWord;
        List<int> orderOfLetters;
        BaseCircle baseCircle;
        SpriteFont letterFont;
        SpriteFont wordListFont;
        Texture2D letterTexture;
        GraphicsDevice GraphicsDevice;
        string baseWord = "";
        public string chosenWord = "";
        Random gen = new Random();
        public List<string> correctWords = new List<string>();
        public void Reshuffle()
        { 
            List<int> positions = new List<int>();
            for (int i = 0; i < baseWord.Length; i++) 
            {
                positions.Add(i);
            }
            for (int i = 0; i < baseWord.Length; i++)
            {
                int num = gen.Next(0, positions.Count);
                letters[i].Position = baseCircle.innerPoints[positions[num]] - new Vector2(letterTexture.Width / 2, letterTexture.Height / 2);
                positions.RemoveAt(num);
            }
        }

        public BaseGameObject(string word,Texture2D baseCircleTexture,Texture2D letterTexture,GraphicsDevice GraphicsDevice,SpriteFont letterFont,SpriteFont wordListFont)
        {
            word = word.ToUpper();
            baseWord = word;
            this.letterFont = letterFont;
            this.wordListFont = wordListFont;
            baseCircle = new BaseCircle(baseCircleTexture, new Vector2(0, GraphicsDevice.Viewport.Height - baseCircleTexture.Height), new Color(Color.LightSteelBlue, 235), GraphicsDevice, letterTexture.Width / 2,word.Length);
            letters = new List<Letter>();
            this.letterTexture = letterTexture;
            this.GraphicsDevice = GraphicsDevice;
            for (int i = 0; i < word.Length; i++)
            {
                letters.Add(new Letter(letterTexture, baseCircle.innerPoints[i] - new Vector2(letterTexture.Width / 2, letterTexture.Height / 2), Color.TransparentBlack, word[i], letterFont, GraphicsDevice, Color.OrangeRed));
            }
            Reshuffle();
            currentWord = "";
            orderOfLetters = new List<int>();
        }

        public void Update(MouseState mouse)
        {
            for (int i = 0; i < letters.Count; i++)
            {
                letters[i].Update(mouse);
            }
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
            currentWord = "";
            for (int i = 0; i < orderOfLetters.Count; i++)
            {
                if (letters[orderOfLetters[i]].didPass || letters[orderOfLetters[i]].isClicked)
                {
                    currentWord += letters[orderOfLetters[i]].letter;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            baseCircle.Draw(spriteBatch);

            for (int i = 0; i < letters.Count; i++)
            {
                letters[i].DrawLine(spriteBatch);
            }
            for (int i = 0; i < letters.Count; i++)
            {
                letters[i].Draw(spriteBatch);
            }
            for(int x = 0;x<correctWords.Count/25;x++)
            {
                for(int y = 0;y<correctWords.Count;y++)
                {
//   spriteBatch.DrawString(wordListFont,correctWords[i],)
                }
            }
            Vector2 sizeOfText = letterFont.MeasureString(currentWord);
            spriteBatch.DrawString(letterFont, currentWord, baseCircle.Position + new Vector2(baseCircle.Radius, -60) - (sizeOfText / 2), Color.Black);
        }
    }
}