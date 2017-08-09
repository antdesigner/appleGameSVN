//非架构
; (function ($) {
    var aspect = 5.7 / 8.8;//扑克宽和高比例
    function PaintBoder(canv, x, y, h,poker) {
        var boder = new Boder(canv, h);
        boder.X = x;
        boder.Y = y;
        var c = canv.getContext("2d");
        c.save()
        c.lineWidth = 1;
        boder.Y = y + c.lineWidth;
        c.strokeStyle = "green";
        c.fillStyle = "white";
        if (poker.Name.length==0) {
            c.fillStyle = "darkgrey";
            c.strokeStyle = "white";
        }
        c.beginPath();
        //画top boder
        var r = boder.width / 8;
        c.moveTo(boder.X + r, boder.Y);
        c.lineTo(boder.X + boder.width - r, boder.Y);
        //画top right 圆角
        c.arcTo(boder.X + boder.width, boder.Y, boder.X + boder.width, boder.Y + r, r)
        //left boder
        c.lineTo(boder.X + boder.width, boder.Y + boder.height - r);
        //bottom left radius
        c.arcTo(boder.X + boder.width, boder.Y + boder.height, boder.X + boder.width - r, boder.Y + boder.height, r)
        //bottom boder
        c.lineTo(boder.X + r, boder.Y + boder.height);
        //bottom right radius
        c.arcTo(boder.X, boder.Y + boder.height, boder.X, boder.Y + boder.height - r, r)
        //right boder
        c.lineTo(boder.X, boder.Y + r);
        //top right radius
        c.arcTo(boder.X, boder.Y, boder.X + r, boder.Y, r);
        c.closePath();
        c.stroke();
        c.fill();
        c.restore();
        return boder;
    }//画边框
    function PaintCardColor(boder, Poker, deg) {
            //字体大小
            var fontSize = boder.height / 2.5;
            //颜色
            var fontColor = GetPaintColor(Poker);
            //点数
            var colorImage = "";
           switch (Poker.CardColor) {
               case "h" :
                   colorImage = "♥";
                   break;
               case "s" :
                   colorImage = "♠";
                   break;
               case "d" :
                   colorImage = "♣";
                   break;
               case "c":
                   colorImage = "♦";
                   break;
               case "z":
                   colorImage = "☻";
               case "y":
                   colorImage = "☻";
                   break;
               default:
                   colorImage = "";
           }
           var canvasContext = boder.canvasContext;
           var fontSize = boder.width * 0.4;
           if (Poker.CardColor=="h") {
               var fontSize = boder.width * 0.38;
           }
            var x = boder.X + 2;
            var y = boder.Y + boder.width * 0.5;//?
            canvasContext.font = (fontSize + "px Arial");
            canvasContext.textAlign = "left";
            canvasContext.textBaseline = "hanging";
            canvasContext.fillStyle = fontColor;
            canvasContext.fillText(colorImage, x, y);
    }
    function PaintName(boder, Poker) {
        //字体大小
        var fontSize = boder.width * 0.4;
        var fontWeight = "normal";
        if (Poker.Name=="10") {
            fontSize = boder.width * 0.35;
            fontWeight = "bolder";
        }
       
        //颜色
        var fontColor = GetPaintColor(Poker);
        //点数
        var name = Poker.Name;
        //画布
        var canvasContext = boder.canvasContext;
        //位置

        var x = boder.X+1;
        var y = boder.Y+5;//?
        canvasContext.font = (fontWeight+" "+fontSize + "px Arial");
        canvasContext.textAlign = "left";
        canvasContext.textBaseline = "hanging";
        canvasContext.fillStyle = fontColor;
        canvasContext.fillText(name, x, y);
    }//画点数
    function GetPaintColor(Poker) {
        var color = "red";
        switch (Poker.CardColor) {
           //hearts红桃、spades黑桃、diamonds梅花,clubs方块
            case "h": color = "red";
                break;
            case "c": color = "red";
                break;
            case "s": color = "black";
                break;
            case "d": color = "black";
                break;
            case "y": color = "black";
                break;
            default: color = "red";
        }
        return color;
    }//画花色
    function Boder(canv, h) {
        this.canvasContext = canv.getContext("2d");
        this.height = h;
        this.width = this.height * aspect ;
        this.X;
        this.Y;
    }//边框对象
    var drawPoker = function (canv,x, y, poker,h,deg) {
        var context = canv.getContext("2d");
        context.save();
            //取中心
        var centerx = x + h * aspect / 2;
        var centery = y + h / 2;
        context.globalCompositeOperation = "source-over";//source-over
            context.translate(centerx, centery);
            //旋转
            context.rotate(deg* Math.PI / 180);
            context.translate(-centerx, -centery);
            //还原中心
            //画边框

            var boder = PaintBoder(canv, x, y, h, poker);
       PaintName(boder, poker);

           PaintCardColor(boder, poker, deg);

           context.translate(centerx, centery);
           context.rotate(180 * Math.PI / 180);
           context.translate(-centerx, -centery);
           PaintName(boder, poker);
           PaintCardColor(boder, poker, deg + 180);
           //context.translate(-centerx, -centery);
           context.restore();

    };
   function toRight (pokers, pokerHeight, deg, pile, x, y,canv) {
            var pokerWidth = pokerHeight*aspect;
            var space = pokerWidth * 0.7;
            var xx = x;
            var yy = y;
            $.each(pokers, function (i, n) {
                drawPoker(canv, xx, yy, n, pokerHeight, deg);
                    if (pile == true) {
                        xx = xx + space;
                    } else {
                        xx = xx + pokerWidth * 1.1;
                    }
            })

    };
    function toLeft(pokers, pokerHeight, deg, pile, x, y, canv) {
        var pokerWidth = pokerHeight * aspect;
        var space = pokerWidth * 0.7;
        var xx = x - (space*(pokers.length-1))-pokerWidth;
        var yy = y;
        $.each(pokers, function (i, n) {
            drawPoker(canv, xx, yy, n, pokerHeight, deg);
            if (pile == true) {
                xx = xx + space;
            } else {
                xx = xx +pokerWidth * 1.1;
            }
        })

    };
   function toBottom(pokers, pokerHeight, deg, pile, x, y, canv) {
       var pokerWidth = pokerHeight * aspect;
       var space = pokerWidth * 0.7;
       var xx = x;
       var yy = y;
       $.each(pokers, function (i, n) {
           drawPoker(canv, xx, yy, n, pokerHeight, deg);
           if (pile == true) {
               yy = yy + space;
           } else {
               yy = yy + pokerWidth * 1.1;
               }
       })

    };
   var methods = {
       init: function (options) {
           var defaults = {
               pokers: [{ Name: "error", CardColor: "h" }],
               pokerHeight: 75,
               deg: 0,
               pile: true,
               x: 0,
               y: 0,
           }
           var settings = $.extend({}, defaults, options);
           if (settings.direction=="toRight") {
               toRight(settings.pokers,
                   settings.pokerHeight,
                   settings.deg,
                   settings.pile,
                   settings.x,
                   settings.y,
                   this.get(0))
           } else if (settings.direction == "toBottom") {
               toBottom(settings.pokers,
                   settings.pokerHeight,
                   settings.deg,
                   settings.pile,
                   settings.x,
                   settings.y,
                   this.get(0))
           } else if (settings.direction == "toLeft") {
               toLeft(settings.pokers,
                   settings.pokerHeight,
                   settings.deg,
                   settings.pile,
                   settings.x,
                   settings.y,
                   this.get(0))
           }else {
               toRight(settings.pokers,
                   settings.pokerHeight,
                   settings.deg,
                   settings.pile,
                   settings.x,
                   settings.y,
                   this.get(0))
           }
         
       },
       toRight: function (pokers, pokerHeight, deg, pile, x, y) {
           var canv = this.get(0);
           toRight(pokers, pokerHeight, deg, pile, x, y, canv);
       },
       toBottom: function (pokers, pokerHeight, deg, pile, x, y) {
           var canv = this.get(0);
           toBottom(pokers, pokerHeight, deg, pile, x, y, canv);
       },
       toLeft: function (pokers, pokerHeight, deg, pile, x, y) {
           var canv = this.get(0);
           toRight(pokers, pokerHeight, deg, pile, x, y, canv);
       }
    };
   $.fn.drawPoker = function (direction,pokers, pokerHeight, deg, pile, x, y) {
            var method = arguments[0];

            if (methods[method]) {   
                method = methods[method];
                arguments = Array.prototype.slice.call(arguments, 1);
            } else if (typeof (method) == 'object' || !method) {
                method = methods.init;
            } else {
                $.error('Mehod ' + method + ' does not exist on this object');
                return this;
            }
                return method.apply(this, arguments);
        };
    })(jQuery);