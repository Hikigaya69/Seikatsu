import { useEffect, useState } from "react";
import axios from "axios";

import HomeNavbar from "../components/home/HomeNavbar";
import HeroBanner from "../components/home/HeroBanner";
import CategoryFilter from "../components/home/CategoryFilter";
import ProductGrid from "../components/home/ProductGrid";
import Footer from "../components/Footer";

export default function Home() {

  const [products, setProducts] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState("All");

  useEffect(() => {

    axios
      .get("https://localhost:7115/api/Product/productforindex")
      .then((res) => {

        setProducts(res.data.data);

      });

  }, []);

  const categories = [
    "All",
    ...new Set(products.map((p) => p.category))
  ];

  const filteredProducts =
    selectedCategory === "All"
      ? products
      : products.filter((p) => p.category === selectedCategory);

  return (

    <div className="min-h-screen">

      <HomeNavbar />

      <div className="p-7">
        <HeroBanner />
      </div>

      <div className="max-w-7xl mx-auto px-6">

        <CategoryFilter
          categories={categories}
          selected={selectedCategory}
          onSelect={setSelectedCategory}
        />

        <ProductGrid products={filteredProducts} />

      </div>

      <Footer />

    </div>

  );
}