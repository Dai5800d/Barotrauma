﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Subsurface
{
    [Flags]
    public enum Alignment 
    { 
        CenterX = 1, Left = 2, Right = 4, CenterY = 8, Top = 16, Bottom = 32 ,
        TopRight = (Top | Right), TopLeft = (Top | Left), TopCenter = (CenterX | Top),
        Center = (CenterX | CenterY),
        BottomRight = (Bottom | Right), BottomLeft = (Bottom | Left), BottomCenter = (CenterX | Bottom)
    }
    

    class GUIMessage
    {
        ColoredText coloredText;
        Vector2 pos;

        float lifeTime;

        Vector2 size;


        public string Text
        {
            get { return coloredText.text; }
        }

        public Color Color
        {
            get { return coloredText.color; }
        }

        public Vector2 Pos
        {
            get { return pos; }
            set { pos = value; }
        }

        public Vector2 Size
        {
            get { return size; }
        }


        public float LifeTime
        {
            get { return lifeTime; }
            set { lifeTime = value; }
        }
        
        public GUIMessage(string text, Color color, Vector2 position, float lifeTime)
        {
            coloredText = new ColoredText(text, color);
            pos = position;
            this.lifeTime = lifeTime;

            size = GUI.Font.MeasureString(text);
        }
    }

    class GUI
    {
        public static GUIStyle style;

        static Texture2D t;
        public static SpriteFont Font, SmallFont;


        private static GraphicsDevice graphicsDevice;
        

        private static List<GUIMessage> messages = new List<GUIMessage>();


        private static Sound[] sounds;


        public static void LoadContent(GraphicsDevice graphics)
        {
            graphicsDevice = graphics;

            sounds = new Sound[2];
            sounds[0] = Sound.Load("Content/Sounds/UI/UImsg.ogg");

            // create 1x1 texture for line drawing
            t = new Texture2D(graphicsDevice, 1, 1);
            t.SetData<Color>(
                new Color[] { Color.White });// fill the texture with white
            
            style = new GUIStyle("Content/UI/style.xml");
        }

        public static void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color clr, float depth = 0.0f)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            
            sb.Draw(t,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                clr, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                depth);
        }

        public static void DrawRectangle(SpriteBatch sb, Vector2 start, Vector2 size, Color clr, bool isFilled = false, float depth = 0.0f)
        {
            if (isFilled)
            {
                sb.Draw(t, new Rectangle((int)start.X,(int)start.Y,(int)size.X,(int)size.Y),null,clr);
            }
            else
            {
                Vector2 p2 = new Vector2(start.X + size.X, start.Y);
                Vector2 p4 = new Vector2(start.X, start.Y + size.Y);

                DrawLine(sb, start, p2, clr, depth);
                DrawLine(sb, p2, start + size, clr, depth);
                DrawLine(sb, start + size, p4, clr, depth);
                DrawLine(sb, p4, start, clr, depth);
            }
        }

        public static void DrawRectangle(SpriteBatch sb, Rectangle rect, Color clr, bool isFilled = false, float depth = 0.0f)
        {
            if (isFilled)
            {
                sb.Draw(t, rect, null, clr);
            }
            else
            {
                Vector2 p1 = new Vector2(rect.X, rect.Y);
                Vector2 p2 = new Vector2(rect.X + rect.Width, rect.Y);
                Vector2 p3 = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);                
                Vector2 p4 = new Vector2(rect.X, rect.Y + rect.Height);

                DrawLine(sb, p1, p2, clr, depth);
                DrawLine(sb, p2, p3, clr, depth);
                DrawLine(sb, p3, p4, clr, depth);
                DrawLine(sb, p4, p1, clr, depth);
            }
        }

        public static Texture2D CreateCircle(int radius)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(graphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        public static Texture2D CreateCapsule(int radius, int height)
        {
            int textureWidth = radius * 2, textureHeight = height + radius * 2;

            Texture2D texture = new Texture2D(graphicsDevice, textureWidth, textureHeight);
            Color[] data = new Color[textureWidth * textureHeight];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (int i = 0; i < 2; i++ )
            {
                for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
                {
                    // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                    int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                    int y = (height-1)*i + (int)Math.Round(radius + radius * Math.Sin(angle));

                    data[y * textureWidth + x] = Color.White;
                }
            }

            for (int y = radius; y<textureHeight-radius; y++)
            {
                data[y * textureWidth] = Color.White;
                data[y * textureWidth + (textureWidth-1)] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        public static Texture2D CreateRectangle(int width, int height)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];

            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            for (int y = 0; y < height; y++)
            {
                data[y * width] = Color.White;
                data[y * width + (width-1)] = Color.White;
            }

            for (int x = 0; x < width; x++)
            {
                data[x] = Color.White;
                data[(height - 1) * width + x] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        public static bool DrawButton(SpriteBatch sb, Rectangle rect, string text, bool isHoldable = false)
        {
            Color color = new Color(200, 200, 200);

            bool clicked = false;

            if (rect.Contains(PlayerInput.GetMouseState.Position))
            {
                clicked = (PlayerInput.GetMouseState.LeftButton == ButtonState.Pressed);

                color = clicked ? new Color(100, 100, 100) : new Color(250, 250, 250);

                if (!isHoldable)
                    clicked = PlayerInput.LeftButtonClicked();
            }

            DrawRectangle(sb, rect, color, true);
            sb.DrawString(Font, text, new Vector2(rect.X + 10, rect.Y + 10), Color.White);

            return clicked;
        }

        public static void Draw(float deltaTime, SpriteBatch spriteBatch, Camera cam)
        {
            spriteBatch.DrawString(Font,
                "FPS: " + (int)Game1.FrameCounter.AverageFramesPerSecond
                + " - Physics: " + Game1.World.UpdateTime
                + " - bodies: " + Game1.World.BodyList.Count,
                new Vector2(10, 10), Color.White);

            
            spriteBatch.DrawString(Font,
                "Camera pos: " + Game1.GameScreen.Cam.Position,
                new Vector2(10, 30), Color.White);
            
            if (Character.Controlled != null && cam!=null) Character.Controlled.DrawHud(spriteBatch, cam);
            if (Game1.NetworkMember != null) Game1.NetworkMember.Draw(spriteBatch);

            DrawMessages(spriteBatch, (float)deltaTime);

            if (GUIMessageBox.messageBoxes.Count>0)
            {
                var messageBox = GUIMessageBox.messageBoxes.Peek();
                if (messageBox != null) messageBox.Draw(spriteBatch);
            }

            
            DebugConsole.Draw(spriteBatch);
        }

        public static void Update(float deltaTime)
        {
            if (GUIMessageBox.messageBoxes.Count > 0)
            {
                var messageBox = GUIMessageBox.messageBoxes.Peek();
                if (messageBox != null)
                {
                    GUIComponent.MouseOn = messageBox;
                    messageBox.Update(deltaTime);
                }
            }
        }

        public static void AddMessage(string message, Color color, float lifeTime = 3.0f, bool playSound = true)
        {
            if (messages.Count>0 && messages[messages.Count-1].Text == message)
            {
                messages[messages.Count - 1].LifeTime = lifeTime;
                return;
            }

            Vector2 currPos = new Vector2(Game1.GraphicsWidth / 2.0f, Game1.GraphicsHeight * 0.7f);
            currPos.Y += messages.Count * 30;

            messages.Add(new GUIMessage(message, color, currPos, lifeTime));
            if (playSound) PlayMessageSound();
        }

        public static void PlayMessageSound()
        {
            sounds[0].Play();
        }

        private static void DrawMessages(SpriteBatch spriteBatch, float deltaTime)
        {
            if (messages.Count == 0) return;

            Vector2 currPos = new Vector2(Game1.GraphicsWidth / 2.0f, Game1.GraphicsHeight * 0.7f);

            int i = 1;
            foreach (GUIMessage msg in messages)
            {
                float alpha = 1.0f;

                if (msg.LifeTime < 1.0f)
                {
                    alpha -= 1.0f - msg.LifeTime;
                }

                msg.Pos = MathUtils.SmoothStep(msg.Pos, currPos, deltaTime*20.0f);

                spriteBatch.DrawString(Font, msg.Text,
                    new Vector2((int)msg.Pos.X, (int)msg.Pos.Y), 
                    msg.Color * alpha, 0.0f,
                    new Vector2((int)(0.5f * msg.Size.X), (int)(0.5f * msg.Size.Y)), 1.0f, SpriteEffects.None, 0.0f);

                currPos.Y += 30.0f;

                messages[0].LifeTime -= deltaTime/i;

                i++;
            }
            
            if (messages[0].LifeTime <= 0.0f) messages.Remove(messages[0]);
        }
    }
}
