using DylanMonoGameIntro;
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
        KeyboardState keyboard;
        KeyboardState previousKeyboard;
        Stringscape stringscape;
        Sprite reshuffleButton;
        MouseState previousMouse;
        Random gen = new Random();
        Texture2D baseCircleTexture;
        Texture2D letterTexture;
        Texture2D leftRightArrowTexture;
        Texture2D upDownArrowTexture;
        Texture2D selectDot;
        SpriteFont letterFont;
        SpriteFont definitionFont;
        SpriteFont wordListFont;
        SpriteFont titleFont;
        Dictionary<int, string> words = new Dictionary<int, string>();
        Sprite casualOptionsButton;
        Sprite timedOptionsButton;
        Sprite backArrow;
        Sprite topSelectDot;
        Sprite bottomSelectDot;
        Textbox amountBox;


        enum ScreenState
        {
            TitleScreen,
            CasualOptions,
            TimedOptions,
            Game,
            EndOfRoundOptions
        }
        ScreenState GameState = ScreenState.TitleScreen;

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
                    words.Add(index, word);
                    index++;
                }
            }

            spriteBatch = new SpriteBatch(GraphicsDevice);

            baseCircleTexture = Content.Load<Texture2D>("nonBlurryCircleScaled");
            letterTexture = Content.Load<Texture2D>("LetterCircle");
            leftRightArrowTexture = Content.Load<Texture2D>("arrow");
            upDownArrowTexture = Content.Load<Texture2D>("upArrow");
            selectDot = Content.Load<Texture2D>("selectDot");
            letterFont = Content.Load<SpriteFont>("font");
            wordListFont = Content.Load<SpriteFont>("WordListFont");
            definitionFont = Content.Load<SpriteFont>("definitionFont");
            titleFont = Content.Load<SpriteFont>("titleFont");

            var casualPixel = new Texture2D(GraphicsDevice, 1, 1);
            casualPixel.SetData(new[] { Color.White });
            casualOptionsButton = new Sprite(casualPixel, new Vector2(GraphicsDevice.Viewport.Width / 2 - (casualPixel.Width * wordListFont.MeasureString("- Casual").X / 2), 600), Color.TransparentBlack, GraphicsDevice) { Scale = wordListFont.MeasureString("- Casual") };

            var timedPixel = new Texture2D(GraphicsDevice, 1, 1);
            timedPixel.SetData(new[] { Color.White });
            timedOptionsButton = new Sprite(timedPixel, new Vector2(GraphicsDevice.Viewport.Width / 2 - (timedPixel.Width * wordListFont.MeasureString("- Timed").X / 2), 750), Color.TransparentBlack, GraphicsDevice) { Scale = wordListFont.MeasureString("- Timed") };

            backArrow = new Sprite(leftRightArrowTexture, new Vector2(10, 10), Color.Black, GraphicsDevice);

            topSelectDot = new Sprite(selectDot, new Vector2(GraphicsDevice.Viewport.Width / 2 - selectDot.Width, 450), Color.White, GraphicsDevice);
            bottomSelectDot = new Sprite(selectDot, new Vector2(GraphicsDevice.Viewport.Width / 2 - selectDot.Width, 550), Color.White, GraphicsDevice);
            
            amountBox = new Textbox(new Vector2(GraphicsDevice.Viewport.Width/2 + 150, 475),Color.White,wordListFont,GraphicsDevice);

            string baseWord = "";
            while (baseWord == "")
            {
                int wordIndex = gen.Next(0, words.Count);
                if (words[wordIndex].Length >= 7 && words[wordIndex].Length <= 8)
                {
                    baseWord = words[wordIndex];
                }
            }
            stringscape = new Stringscape(baseWord, baseCircleTexture, letterTexture, leftRightArrowTexture, upDownArrowTexture, GraphicsDevice, letterFont, wordListFont, definitionFont);
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
            keyboard = Keyboard.GetState();

            switch (GameState)
            {
                case ScreenState.TitleScreen:
                    if (casualOptionsButton.Bounds.Contains(mouse.Position))
                    {
                        casualOptionsButton.Color = Color.OrangeRed;
                        if (mouse.LeftButton == ButtonState.Pressed)
                        {
                            GameState = ScreenState.CasualOptions;
                        }
                    }
                    else
                    {
                        casualOptionsButton.Color = Color.TransparentBlack;
                    }

                    if (timedOptionsButton.Bounds.Contains(mouse.Position))
                    {
                        timedOptionsButton.Color = Color.OrangeRed;
                        if (mouse.LeftButton == ButtonState.Pressed)
                        {
                            GameState = ScreenState.TimedOptions;
                        }
                    }
                    else
                    {
                        timedOptionsButton.Color = Color.TransparentBlack;
                    }
                    break;

                case ScreenState.CasualOptions:
                    if (mouse.LeftButton == ButtonState.Pressed && topSelectDot.Bounds.Contains(mouse.Position))
                    {
                        topSelectDot.Color = Color.Black;
                        bottomSelectDot.Color = Color.White;
                    }
                    else if (mouse.LeftButton == ButtonState.Pressed && bottomSelectDot.Bounds.Contains(mouse.Position))
                    {
                        topSelectDot.Color = Color.White;
                        bottomSelectDot.Color = Color.Black;
                    }
                    amountBox.Update(mouse, keyboard, previousKeyboard);

                    if (mouse.LeftButton == ButtonState.Pressed && backArrow.Bounds.Contains(mouse.Position))
                    {
                        topSelectDot.Color = Color.White;
                        bottomSelectDot.Color = Color.White;
                        amountBox.currentWord = " ";
                        GameState = ScreenState.TitleScreen;
                    }
                    break;

                case ScreenState.TimedOptions:
                    if (mouse.LeftButton == ButtonState.Pressed && topSelectDot.Bounds.Contains(mouse.Position))
                    {
                        topSelectDot.Color = Color.Black;
                        bottomSelectDot.Color = Color.White;
                    }
                    else if (mouse.LeftButton == ButtonState.Pressed && bottomSelectDot.Bounds.Contains(mouse.Position))
                    {
                        topSelectDot.Color = Color.White;
                        bottomSelectDot.Color = Color.Black;
                    }
                    amountBox.Update(mouse, keyboard, previousKeyboard);

                    if (mouse.LeftButton == ButtonState.Pressed && backArrow.Bounds.Contains(mouse.Position))
                    {
                        topSelectDot.Color = Color.White;
                        bottomSelectDot.Color = Color.White;
                        amountBox.currentWord = " ";
                        GameState = ScreenState.TitleScreen;
                    }
                    break;

                case ScreenState.Game:
                    if (Math.Pow(reshuffleButton.Position.X + reshuffleButton.Radius - mouse.Position.X, 2) + Math.Pow(reshuffleButton.Position.Y + reshuffleButton.Radius - mouse.Position.Y, 2) <= Math.Pow(reshuffleButton.Radius, 2) && mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton != ButtonState.Pressed)
                    {
                        stringscape.Reshuffle();
                    }

                    stringscape.Update(mouse);
                    if (stringscape.chosenWord.Length > 2)
                    {
                        bool containsWord = false;
                        for (int i = 0; i < stringscape.CorrectWords.Count; i++)
                        {
                            if (stringscape.CorrectWords[i].word == stringscape.chosenWord)
                            {
                                containsWord = true;
                            }
                        }

                        if (words.ContainsValue(stringscape.chosenWord) && !containsWord)
                        {
                            stringscape.CorrectWords.Add(new CorrectWord(stringscape.chosenWord, wordListFont));
                        }
                        stringscape.chosenWord = "";

                    }
                    break;

                case ScreenState.EndOfRoundOptions:
                    break;
            }
            previousMouse = mouse;
            previousKeyboard = keyboard;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            switch (GameState)
            {
                case ScreenState.TitleScreen:
                    spriteBatch.DrawString(titleFont, "Stringscapes", new Vector2((GraphicsDevice.Viewport.Width / 2) - (titleFont.MeasureString("Stringscapes").X / 2), 50), Color.Black);
                    casualOptionsButton.Draw(spriteBatch);
                    spriteBatch.DrawString(wordListFont, "- Casual", casualOptionsButton.Position, Color.Black);
                    timedOptionsButton.Draw(spriteBatch);
                    spriteBatch.DrawString(wordListFont, "- Timed", timedOptionsButton.Position, Color.Black);
                    break;

                case ScreenState.CasualOptions:
                    spriteBatch.DrawString(wordListFont, "Casual Options:", new Vector2((GraphicsDevice.Viewport.Width / 2) - (wordListFont.MeasureString("Casual Options:").X / 2), 50), Color.Black);
                    topSelectDot.Draw(spriteBatch);
                    spriteBatch.DrawString(definitionFont, "pages:", new Vector2(topSelectDot.Position.X + topSelectDot.Image.Width, topSelectDot.Position.Y), Color.Black);
                    bottomSelectDot.Draw(spriteBatch);
                    spriteBatch.DrawString(definitionFont, "words:", new Vector2(bottomSelectDot.Position.X + bottomSelectDot.Image.Width, bottomSelectDot.Position.Y), Color.Black);
                    backArrow.Draw(spriteBatch);
                    amountBox.Draw(spriteBatch);
                    break;

                case ScreenState.TimedOptions:
                    spriteBatch.DrawString(wordListFont, "Timed Options:", new Vector2((GraphicsDevice.Viewport.Width / 2) - (wordListFont.MeasureString("Timed Options:").X / 2), 50), Color.Black);
                    topSelectDot.Draw(spriteBatch);
                    spriteBatch.DrawString(definitionFont, "minutes:", new Vector2(topSelectDot.Position.X + topSelectDot.Image.Width, topSelectDot.Position.Y), Color.Black);
                    bottomSelectDot.Draw(spriteBatch);
                    spriteBatch.DrawString(definitionFont, "seconds:", new Vector2(bottomSelectDot.Position.X + bottomSelectDot.Image.Width, bottomSelectDot.Position.Y), Color.Black);
                    backArrow.Draw(spriteBatch);
                    amountBox.Draw(spriteBatch);
                    break;

                case ScreenState.Game:
                    stringscape.Draw(spriteBatch);
                    reshuffleButton.Draw(spriteBatch);
                    break;

                case ScreenState.EndOfRoundOptions:
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}