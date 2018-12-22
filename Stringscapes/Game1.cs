﻿using DylanMonoGameIntro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace Stringscapes
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MouseState mouse;
        Stringscape stringscape;
        Sprite reshuffleButton;
        MouseState previous;
        Random gen = new Random();
        SpriteFont wordListFont;
        Dictionary<int, string> words = new Dictionary<int, string>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 1100;
            graphics.PreferredBackBufferWidth = 2000;
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            using (StreamReader stream = new StreamReader("words.txt"))
            {
                int index = 0;
                string word = "";
                while ((word = stream.ReadLine()) != null)
                {
                    words.Add(index,word);
                    index++;
                }
            }            

            spriteBatch = new SpriteBatch(GraphicsDevice);            
            Texture2D baseCircleTexture = Content.Load<Texture2D>("nonBlurryCircleScaled");
            Texture2D letterTexture = Content.Load<Texture2D>("LetterCircle");
            Texture2D leftRightArrowTexture = Content.Load<Texture2D>("arrow");
            Texture2D upDownArrowTexture = Content.Load<Texture2D>("upArrow");
            SpriteFont letterFont = Content.Load<SpriteFont>("font");
            wordListFont = Content.Load<SpriteFont>("WordListFont");
            SpriteFont definitionFont = Content.Load<SpriteFont>("definitionFont");
            string baseWord = "";
            while (baseWord == "")
            {
                int wordIndex = gen.Next(0,words.Count);
                if(words[wordIndex].Length >=7 && words[wordIndex].Length <=8)
                {
                    baseWord = words[wordIndex];
                }
            }
            stringscape = new Stringscape(baseWord, baseCircleTexture, letterTexture,leftRightArrowTexture,upDownArrowTexture, GraphicsDevice, letterFont, wordListFont, definitionFont);
            reshuffleButton = new Sprite(Content.Load<Texture2D>("cycle"), Vector2.Zero, Color.LightGray, GraphicsDevice)
            {
                Scale = new Vector2(.25f)
            };

        }

        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            mouse = Mouse.GetState();
            //System.Diagnostics.Debug.WriteLine($"Mouse: ({mouse.X}, {mouse.Y})");
            if (Math.Pow(reshuffleButton.Position.X + reshuffleButton.Radius - mouse.Position.X, 2) + Math.Pow(reshuffleButton.Position.Y + reshuffleButton.Radius - mouse.Position.Y, 2) <= Math.Pow(reshuffleButton.Radius,2) && mouse.LeftButton == ButtonState.Pressed && previous.LeftButton != ButtonState.Pressed)
            {
               stringscape.Reshuffle();
            }
            
            stringscape.Update(mouse);
            if(stringscape.chosenWord.Length > 2)
            {
                bool containsWord = false;
                for(int i = 0;i<stringscape.CorrectWords.Count;i++)
                {
                    if(stringscape.CorrectWords[i].word == stringscape.chosenWord)
                    {
                        containsWord = true;
                    }
                }

                if(words.ContainsValue(stringscape.chosenWord) && !containsWord)
                {
                    stringscape.CorrectWords.Add(new CorrectWord(stringscape.chosenWord,wordListFont));
                }
                stringscape.chosenWord = "";
            }
            previous = mouse;
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            stringscape.Draw(spriteBatch);
            reshuffleButton.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}