using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using XnaCards;

namespace ProgrammingAssignment6
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WINDOW_WIDTH = 800;
        const int WINDOW_HEIGHT = 600;

        // max valid blackjack score for a hand
        const int MAX_HAND_VALUE = 21;
        const int DEALER_STAND_SCORE = 17;

        // deck and hands
        Deck deck;
        List<Card> dealerHand = new List<Card>();
        List<Card> playerHand = new List<Card>();

        // hand placement
        const int TOP_CARD_OFFSET = 100;
        const int HORIZONTAL_CARD_OFFSET = 150;
        const int VERTICAL_CARD_SPACING = 125;

        // messages
        SpriteFont messageFont;
        const string SCORE_MESSAGE_PREFIX = "Score: ";
        Message playerScoreMessage;
        List<Message> messages = new List<Message>();

        // message placement
        const int SCORE_MESSAGE_TOP_OFFSET = 25;
        const int HORIZONTAL_MESSAGE_OFFSET = HORIZONTAL_CARD_OFFSET;
        Vector2 winnerMessageLocation = new Vector2(WINDOW_WIDTH / 2,
            WINDOW_HEIGHT / 2);

        // menu buttons
        Texture2D quitButtonSprite;
        List<MenuButton> menuButtons = new List<MenuButton>();

        // menu button placement
        const int TOP_MENU_BUTTON_OFFSET = TOP_CARD_OFFSET;
        const int QUIT_MENU_BUTTON_OFFSET = WINDOW_HEIGHT - TOP_CARD_OFFSET;
        const int HORIZONTAL_MENU_BUTTON_OFFSET = WINDOW_WIDTH / 2;
        const int VERTICAL_MENU_BUTTON_SPACING = 125;

        // use to detect hand over when player and dealer didn't hit
        bool playerHit = false;
        bool dealerHit = false;

        // game state tracking
        static GameState currentState = GameState.WaitingForPlayer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution and show mouse
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // create and shuffle deck
            deck = new Deck(Content, 0, 0);
            deck.Shuffle();

            // first player card
            Card playerCard1 = deck.TakeTopCard();
            playerCard1.FlipOver();
            playerCard1.X = HORIZONTAL_CARD_OFFSET;
            playerCard1.Y = TOP_CARD_OFFSET;
            playerHand.Add(playerCard1);

            // first dealer card            
            Card dealerCard1 = deck.TakeTopCard();
            dealerCard1.X = WINDOW_WIDTH - HORIZONTAL_CARD_OFFSET;
            dealerCard1.Y = TOP_CARD_OFFSET;
            dealerHand.Add(dealerCard1);

            // second player card
            Card playerCard2 = deck.TakeTopCard();
            playerCard2.FlipOver();
            playerCard2.X = HORIZONTAL_CARD_OFFSET;
            playerCard2.Y = TOP_CARD_OFFSET + playerHand.Count() * VERTICAL_CARD_SPACING;
            playerHand.Add(playerCard2);


            // second dealer card   
            Card dealerCard2 = deck.TakeTopCard();
            dealerCard2.FlipOver();
            dealerCard2.X = WINDOW_WIDTH - HORIZONTAL_CARD_OFFSET;
            dealerCard2.Y = TOP_CARD_OFFSET + dealerHand.Count() * VERTICAL_CARD_SPACING;
            dealerHand.Add(dealerCard2);


            // load sprite font, create message for player score and add to list
            messageFont = Content.Load<SpriteFont>("Arial24");
            playerScoreMessage = new Message(SCORE_MESSAGE_PREFIX + GetBlackjackScore(playerHand).ToString(),
                messageFont,
                new Vector2(HORIZONTAL_MESSAGE_OFFSET, SCORE_MESSAGE_TOP_OFFSET));
            messages.Add(playerScoreMessage);

            // load quit button sprite for later use
            quitButtonSprite = Content.Load<Texture2D>("quitbutton");

            // create hit button and add to list
            menuButtons.Add(new MenuButton(Content.Load<Texture2D>("hitbutton"),
                            new Vector2(HORIZONTAL_MENU_BUTTON_OFFSET, TOP_MENU_BUTTON_OFFSET),
                            GameState.PlayerHitting));

            // create stand button and add to list
            menuButtons.Add(new MenuButton(Content.Load<Texture2D>("standbutton"),
                            new Vector2(HORIZONTAL_MENU_BUTTON_OFFSET, TOP_MENU_BUTTON_OFFSET + VERTICAL_MENU_BUTTON_SPACING),
                            GameState.WaitingForDealer));

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // update menu buttons as appropriate
            MouseState mouseState = Mouse.GetState();
            foreach (MenuButton btn in menuButtons)
            {
                if (currentState == GameState.WaitingForPlayer || currentState == GameState.DisplayingHandResults)
                {
                    btn.Update(mouseState);
                }
            }

            // game state-specific processing
            switch (currentState)
            {
                case GameState.PlayerHitting:
                    // get a new card for the player
                    Card playerCard = deck.TakeTopCard();
                    playerCard.FlipOver();
                    playerCard.X = HORIZONTAL_CARD_OFFSET;
                    playerCard.Y = TOP_CARD_OFFSET + playerHand.Count() * VERTICAL_MENU_BUTTON_SPACING;
                    playerHand.Add(playerCard);

                    playerHit = true;

                    // adapt score and state
                    playerScoreMessage.Text = SCORE_MESSAGE_PREFIX + GetBlackjackScore(playerHand).ToString();
                    ChangeState(GameState.WaitingForDealer);
                    break;
                case GameState.WaitingForDealer:
                    // decide for dealer to stand or hit
                    if (GetBlackjackScore(dealerHand) < DEALER_STAND_SCORE)
                    {
                        ChangeState(GameState.DealerHitting);
                    }
                    else
                    {
                        ChangeState(GameState.CheckingHandOver);
                    }
                    break;
                case GameState.DealerHitting:
                    // get a new card for the dealer
                    Card dealerCard = deck.TakeTopCard();
                    dealerCard.FlipOver();
                    dealerCard.X = WINDOW_WIDTH - HORIZONTAL_CARD_OFFSET;
                    dealerCard.Y = TOP_CARD_OFFSET + dealerHand.Count() * VERTICAL_MENU_BUTTON_SPACING;
                    dealerHand.Add(dealerCard);

                    dealerHit = true;

                    // set next state
                    ChangeState(GameState.CheckingHandOver);
                    break;
                case GameState.CheckingHandOver:
                    // if either player or dealer busts or both player stand
                    if (GetBlackjackScore(playerHand) > MAX_HAND_VALUE ||
                        GetBlackjackScore(dealerHand) > MAX_HAND_VALUE ||
                        !(dealerHit || playerHit))
                    {
                        string winnerText = "";

                        // if both players bust or have the same score, it's a tie
                        if ((GetBlackjackScore(playerHand) > MAX_HAND_VALUE &&
                             GetBlackjackScore(dealerHand) > MAX_HAND_VALUE) ||
                            (GetBlackjackScore(playerHand) == GetBlackjackScore(dealerHand)))
                        {
                            winnerText = "It's a Tie!";
                        }
                        else if (GetBlackjackScore(playerHand) > MAX_HAND_VALUE)
                        {
                            // if player busts, dealer wins
                            winnerText = "Dealer Won!";
                        }
                        else if (GetBlackjackScore(dealerHand) > MAX_HAND_VALUE) 
                        {
                            // if dealer busts, player wins
                            winnerText = "Player Won!";
                        }
                        else if (GetBlackjackScore(playerHand) < GetBlackjackScore(dealerHand))
                        {
                            // if player has less points, dealer wins
                            winnerText = "Dealer Won!";
                        }
                        else if (GetBlackjackScore(playerHand) > GetBlackjackScore(dealerHand))
                        {
                            // if player has less points, dealer wins
                            winnerText = "Player Won!";
                        }

                        // flip over first dealer card
                        dealerHand[0].FlipOver();

                        // create winner message
                        Message winnerMsg = new Message(winnerText, messageFont, winnerMessageLocation);
                        messages.Add(winnerMsg);

                        // create dealer score
                        Message dealerScoreMessage = new Message(SCORE_MESSAGE_PREFIX + GetBlackjackScore(dealerHand).ToString(),
                                                        messageFont,
                                                        new Vector2(WINDOW_WIDTH-HORIZONTAL_MESSAGE_OFFSET, SCORE_MESSAGE_TOP_OFFSET));
                        messages.Add(dealerScoreMessage);

                        // remove stand and hit buttons and add quit button
                        menuButtons.RemoveAt(0);
                        menuButtons.RemoveAt(0);
                        menuButtons.Add(new MenuButton(quitButtonSprite,
                                        new Vector2(HORIZONTAL_MENU_BUTTON_OFFSET, QUIT_MENU_BUTTON_OFFSET),
                                        GameState.Exiting));
                        ChangeState(GameState.DisplayingHandResults);

                    }
                    else
                    {
                        ChangeState(GameState.WaitingForPlayer);
                    }
                    // reset the hit state of playerhit and dealerhit to false
                    playerHit = false;
                    dealerHit = false;
                    break;
                case GameState.Exiting:
                    // exit the game
                    Exit();
                    break;
                default:
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Goldenrod);

            spriteBatch.Begin();

            // draw hands
            // draw player hand
            foreach (Card card in playerHand)
            {
                card.Draw(spriteBatch);
            }

            // draw dealer hand
            foreach (Card card in dealerHand)
            {
                card.Draw(spriteBatch);
            }


            // draw messages
            foreach(Message msg in messages)
            {
                msg.Draw(spriteBatch);
            }


            // draw menu buttons
            foreach (MenuButton btn in menuButtons)
            {
                btn.Draw(spriteBatch);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Calculates the Blackjack score for the given hand
        /// </summary>
        /// <param name="hand">the hand</param>
        /// <returns>the Blackjack score for the hand</returns>
        private int GetBlackjackScore(List<Card> hand)
        {
            // add up score excluding Aces
            int numAces = 0;
            int score = 0;
            foreach (Card card in hand)
            {
                if (card.Rank != Rank.Ace)
                {
                    score += GetBlackjackCardValue(card);
                }
                else
                {
                    numAces++;
                }
            }

            // if more than one ace, only one should ever be counted as 11
            if (numAces > 1)
            {
                // make all but the first ace count as 1
                score += numAces - 1;
                numAces = 1;
            }

            // if there's an Ace, score it the best way possible
            if (numAces > 0)
            {
                if (score + 11 <= MAX_HAND_VALUE)
                {
                    // counting Ace as 11 doesn't bust
                    score += 11;
                }
                else
                {
                    // count Ace as 1
                    score++;
                }
            }

            return score;
        }

        /// <summary>
        /// Gets the Blackjack value for the given card
        /// </summary>
        /// <param name="card">the card</param>
        /// <returns>the Blackjack value for the card</returns>
        private int GetBlackjackCardValue(Card card)
        {
            switch (card.Rank)
            {
                case Rank.Ace:
                    return 11;
                case Rank.King:
                case Rank.Queen:
                case Rank.Jack:
                case Rank.Ten:
                    return 10;
                case Rank.Nine:
                    return 9;
                case Rank.Eight:
                    return 8;
                case Rank.Seven:
                    return 7;
                case Rank.Six:
                    return 6;
                case Rank.Five:
                    return 5;
                case Rank.Four:
                    return 4;
                case Rank.Three:
                    return 3;
                case Rank.Two:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Changes the state of the game
        /// </summary>
        /// <param name="newState">the new game state</param>
        public static void ChangeState(GameState newState)
        {
            currentState = newState;
        }
    }
}
