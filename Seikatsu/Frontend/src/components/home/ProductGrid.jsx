import { useEffect, useState } from "react";
import axios from "axios";
import ProductCard from "./ProductCard";

export default function ProductGrid() {

  const [products, setProducts] = useState([]);

  useEffect(() => {

    axios
      .get("https://localhost:7115/api/Product/productforindex")
      .then((res) => {

        setProducts(res.data.data);

      });

  }, []);

  return (

    <div className="grid grid-cols-4 gap-8">

      {products.map((p) => (
        <ProductCard key={p.id} product={p} />
      ))}

    </div>

  );
}