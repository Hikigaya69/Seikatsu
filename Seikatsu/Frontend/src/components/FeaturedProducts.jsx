import { useRef } from "react";
import { useNavigate } from "react-router-dom";
import "./FeaturedProducts.css";

import ramen from "../assets/products/ramen.jpg";
import curry from "../assets/products/curry.jpg";
import masala from "../assets/products/masala.jpg";
import drinks from "../assets/products/drinks.jpg";
import koreaSnack from "../assets/products/koreasnackbox.webp";
import thai from "../assets/products/thaichillipaste.jpg";

function FeaturedProducts() {
  const scrollRef = useRef(null);
  const navigate = useNavigate();

  const scroll = (direction) => {
    const { current } = scrollRef;
    if (direction === "left") {
      current.scrollBy({ left: -300, behavior: "smooth" });
    } else {
      current.scrollBy({ left: 300, behavior: "smooth" });
    }
  };

  const products = [
    {
      name: "Instant Ramen Pack",
      price: "¥850",
      image: ramen,
    },
    {
      name: "Premium Curry Sauce Box",
      price: "¥1,200",
      image: curry,
    },
    {
      name: "Indian Spice Collection",
      price: "¥1,800",
      image: masala,
    },
    {
      name: "Japanese Drink Pack",
      price: "¥2,500",
      image: drinks,
    },


    { name: "Korean Snack Box", price: "¥1,100", image: koreaSnack },
  { name: "Thai Chili Paste", price: "¥900", image: thai },
  { name: "Chinese Dumpling Kit", price: "¥1,600", image: thai },
  { name: "Matcha Dessert Set", price: "¥2,200", image: koreaSnack },
  ];

  return (
    <section className="featured-section">
      <div className="featured-header">
        <h2>Featured Products</h2>
        <p>Hand-picked favorites from our collection</p>
      </div>

      <div className="carousel-wrapper">

        <button className="arrow left" onClick={() => scroll("left")}>
          ❮
        </button>

        <div className="product-container" ref={scrollRef}>
          {products.map((product, index) => (
            <div className="product-card" key={index}>
              <img src={product.image} alt={product.name} />

              <div className="product-info">
                <h3>{product.name}</h3>
                <p className="price">{product.price}</p>

                <button
                  className="cart-btn"
                  onClick={() => navigate("/login")}
                >
                  Add to Cart
                </button>
              </div>
            </div>
          ))}
        </div>

        <button className="arrow right" onClick={() => scroll("right")}>
          ❯
        </button>

      </div>
    </section>
  );
}

export default FeaturedProducts;