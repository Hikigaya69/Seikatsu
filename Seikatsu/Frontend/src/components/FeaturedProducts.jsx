import { useEffect, useState } from "react";
import axios from "axios";
import "./FeaturedProducts.css";
import fallbackImg from "../assets/products/ramen.jpg";

export default function FeaturedProducts() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);


useEffect(() => {
  const fetchProducts = async () => {
    try {
      const response = await axios.get(
        "https://localhost:7115/api/Product/productforindex?count=10"
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

      <div className="products-container">
        {products.map((product) => (
          <div className="product-card" key={product.id}>
            <img
              src={product.productImageUrl || fallbackImg}
              alt={product.name}
            />

            <div className="product-info">
              <h3>{product.name}</h3>
              <p>{product.description}</p>
              <div className="price">¥{product.price}</div>
              <button>Add to Cart</button>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}