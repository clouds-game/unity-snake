# %%
import brotli
import gzip
import os
# os.chdir(os.path.dirname(__file__))

# %%
prefix = "Build"
filenames = [n.removesuffix(".br") for n in os.listdir(prefix) if n.endswith(".br")]

# %%
for n in filenames:
  with open(f"{prefix}/{n}.br", "rb") as f:
    data = f.read()
  data = brotli.decompress(data)
  with open(f"{prefix}/{n}", "wb") as f:
    f.write(data)
  with gzip.open(f"{prefix}/{n}.gz", "wb") as f:
    f.write(data)

# %%
# %%
