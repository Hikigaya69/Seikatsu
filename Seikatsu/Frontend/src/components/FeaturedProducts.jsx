import { useEffect, useState } from "react";

import "./FeaturedProducts.css";
import fallbackImg from "../assets/products/ramen.jpg";

import { Swiper, SwiperSlide } from "swiper/react";
import "swiper/css";

import SeasonLoader from "@/components/SeasonLoader/SeasonLoader";
import { Button } from "@/components/ui/button";
import api from "../Utils/api";

export default function FeaturedProducts() {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const response = await api.get(
                    "/Product/productforindex?pageSize=10" // ✅ fixed param
                );

                // ✅ Correct extraction from API
                const items = response.data?.data?.items ?? [];

                setProducts(items);
            } catch (error) {
                console.error("Error fetching products:", error);
                setProducts([]); // ✅ fallback safety
            } finally {
                setLoading(false);
            }
        };

        fetchProducts();
    }, []);

    if (loading) {
        return <SeasonLoader />;
    }

    return (
        <section className="featured-section">
            <h2 className="text-4xl md:text-5xl font-extrabold tracking-tight text-gray-900 mb-10">
                Featured <span className="text-red-500">Products</span>
            </h2>

            <Swiper spaceBetween={20} slidesPerView={4}>
                {(Array.isArray(products) ? products : []).map((product) => (
                    <SwiperSlide key={product.id}>
                        <div className="product-card">
                            <img
                                src={product.productImageUrl || fallbackImg}
                                alt={product.name}
                            />

                            <div className="product-info">
                                <h3>{product.name}</h3>
                                <p>{product.description}</p>

                                <div className="price">₹{product.price}</div>

                                <Button
                                    variant="outline"
                                    className="px-10 py-6 text-lg text-white rounded-xl border-gray-300 bg-rose-500
                                    shadow-[0_6px_0_rgba(0,0,0,0.2)]
                                    hover:shadow-[0_10px_20px_rgba(0,0,0,0.2)]
                                    hover:-translate-y-1
                                    active:translate-y-1 active:shadow-[0_2px_0_rgba(0,0,0,0.2)]
                                    transition-all duration-150"
                                >
                                    Add to Cart
                                </Button>
                            </div>
                        </div>
                    </SwiperSlide>
                ))}
            </Swiper>
        </section>
    );
}