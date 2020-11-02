﻿/*
 * ************************************************************
 *   _     _              _   _                 _ _           
 *  | |   (_)_ __   ___  | | | | __ _ _ __   __| | | ___ _ __ 
 *  | |   | | '_ \ / _ \ | |_| |/ _` | '_ \ / _` | |/ _ \ '__|
 *  | |___| | | | |  __/ |  _  | (_| | | | | (_| | |  __/ |   
 *  |_____|_|_| |_|\___| |_| |_|\__,_|_| |_|\__,_|_|\___|_|   
 *
 * Octoller.LineCommander
 * 25.08.2020
 *  
 ************************************************************** 
 */

using Octoller.OrderLineHandler.ServiceObjects;
using Octoller.OrderLineHandler.Wrappers;
using Octoller.OrderLineHandler.Default;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Octoller.OrderLineHandler.Processor {

    public sealed class InputHandler {

        private Dictionary<string, IOrderHeader> orderHeaders =
            new Dictionary<string, IOrderHeader>();

        private OrderListCreator creator;
        private static Starter starter;

        static InputHandler() => starter = new Starter();

        public InputHandler() {

            creator = new OrderListCreator();
            AddOrder(new HelperHead());
            ///TODO: добавить возможность установки своих разделителей для комманд и аргументов
        }

        public void AddOrder(IOrderHeader header) {

            if (header != null) {

                orderHeaders.Add(header.Key, header);
            }
        }

        public void AddOrderes(params IOrderHeader[] headers) {

            if (headers != null || headers.Length > 0) {

                Array.ForEach<IOrderHeader>(headers, header => {
                    orderHeaders.Add(header.Key, header);
                });
            }
        }

        public void AddOrderRange(IEnumerable<IOrderHeader> headers) {

            if (headers != null || headers.Count() > 0) {

                Array.ForEach<IOrderHeader>(headers.ToArray(), header => {
                    orderHeaders.Add(header.Key, header);
                });
            }
        }

        public IChContext ParseOrderLine(string input, IChContext context) {

            IChContext ansver;

            try {

                ansver = Parse(input, context);
                
            } catch (Exception ex) {

                ansver = context;
                ansver.Complite = false;
                ansver.SetError($"Error: {ex.Message}");

            }

            return ansver;
        }

        private IChContext Parse(string input, IChContext context) {

            var orderQueue = creator.Parse(input);

            if (orderQueue.Count == 0) {

                throw new ArgumentException("Invalid or empty input string.");
            }

            return FollowСhainOrders(orderQueue, context);
        }

        private IChContext FollowСhainOrders(QueueWrap orderQueue, IChContext context) {

            QueueWrap queue = orderQueue;
            ParseElement parseElement = queue.Dequeue();
            IExecution previousLink = starter;
            TransitionSign nextSign = TransitionSign.None;

            while (!parseElement.IsNull) {

                if (orderHeaders.TryGetValue(parseElement.OrderName, out IOrderHeader header)) {

                    IExecution nextLink = new ExecutionСontainer();
                    IOrderHandler curentHandler = header.GetHandler(parseElement.Arguments);

                    if (curentHandler is HelperHandler helpHand) {

                        helpHand.SetArgument(orderHeaders);
                        curentHandler = helpHand;
                    }

                    nextLink.PrepareHandler(curentHandler);
                    previousLink.SetNext(nextLink, nextSign);
                    previousLink = nextLink;
                    nextSign = parseElement.Sign;

                } else {

                    throw new ArgumentException("Input command name error.");
                }

                parseElement = queue.Dequeue();
            }

            return starter.RunHandler(context);
        }
    }
}
