import { useEffect, useState } from "react";
import axios from "axios";
import "./FeaturedProducts.css";
import fallbackImg from "../assets/products/ramen.jpg";
import { Swiper, SwiperSlide } from "swiper/react";
import "swiper/css";

export default function FeaturedProducts() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);


useEffect(() => {
  const fetchProducts = async () => {
    try {
      const response = await axios.get(
        "https://seikatsu-api.onrender.com/api/Product/productforindex?count=10"
      );

      setProducts(response.data.data);
    } catch (error) {
      console.error("Error fetching products:", error);
    } finally {
      setLoading(false);
    }
  };

  fetchProducts();
}, []);

 
 

  if (loading) {
    return <div className="featured-section">Loading products...</div>;
  }

  return (
    <section className="featured-section">
      <h2>Featured Products</h2>

      <Swiper
  spaceBetween={20}
  slidesPerView={4}
>

{products.map((product) => (

<SwiperSlide key={product.id}>

<div className="product-card">

<img
src={product.productImageUrl || fallbackImg}
alt={product.name}
/>

<div className="product-info">

<h3>{product.name}</h3>

<p>{product.description}</p>

<div className="price">¥{product.price}</div>

<button className="cart-btn">Add to Cart</button>

</div>

</div>

</SwiperSlide>

))}

</Swiper>
    </section>
  );
}